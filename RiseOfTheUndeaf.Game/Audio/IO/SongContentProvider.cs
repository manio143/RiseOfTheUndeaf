using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RiseOfTheUndeaf.Audio.Models;
using RiseOfTheUndeaf.Core.Logging;
using Stride.Audio;
using Stride.Core;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using static System.FormattableString;

namespace RiseOfTheUndeaf.Audio.IO
{
    /// <summary>
    /// Provider of operations related to songs data.
    /// </summary>
    public class SongContentProvider
    {
        public const string SongLibraryUrl = "SongLibrary";

        private ContentManager Content { get; }

        /// <summary>
        /// Creates new instance of <see cref="SongContentProvider"/> with reference to the given <paramref name="contentManager"/>.
        /// </summary>
        /// <param name="contentManager">Content manager to use for object database operations.</param>
        public SongContentProvider(ContentManager contentManager) => this.Content = contentManager;

        /// <summary>
        /// Adds a song to the global <see cref="SongLibrary"/> or returns an existing one if the song of that name already exists.
        /// </summary>
        /// <param name="name">Name of the song.</param>
        /// <param name="wavePath">Path to the WAV file.</param>
        /// <param name="compressedBitRate">Target compression bitrate.</param>
        /// <param name="minBpm">Minum BPM for beat detection algorithm.</param>
        /// <param name="maxBpm">Maximum BPM for beat detection algorithm.</param>
        /// <returns>A new or existing song instance that has its data persisted to disk.</returns>
        public Song AddSongToLibrary(string name, string wavePath, BitRate compressedBitRate, int minBpm, int maxBpm)
        {
            SongLibrary library = GetSongLibrary();

            Song song;
            if ((song = library.Songs.FirstOrDefault(song => song.Name == name)) != null)
            {
                this.LogWarning("Song named '{songName}' already exists.", name);
                return song;
            }

            var songId = Guid.NewGuid();

            try
            {
                song = new Song
                {
                    Name = name,
                    Id = songId,
                    Beats = DetectBeats(wavePath, minBpm, maxBpm),
                    Sound = WriteSoundData(wavePath, compressedBitRate, songId)
                };
                Content.Save(SongUrl(songId), song);

                library.songs.Add(songId, song);
                Content.Save(SongLibraryUrl, library);

                return song;
            }
            catch (Exception ex)
            {
                Content.FileProvider.FileDelete(SoundDataUrl(songId));
                Content.FileProvider.FileDelete(SoundUrl(songId));
                Content.FileProvider.FileDelete(SongUrl(songId));
                library.songs.Remove(songId);

                this.LogError(ex, "An error occurred when creating song '{songName}'.", name);
                throw;
            }
        }

        /// <summary>
        /// Perform beat detection on a WAV file.
        /// </summary>
        /// <param name="wavePath">Path to the WAV file.</param>
        /// <param name="minBpm">Minum BPM for beat detection algorithm.</param>
        /// <param name="maxBpm">Maximum BPM for beat detection algorithm.</param>
        /// <remarks>
        /// The max BPM has to be in range (minBPM, 2 * minBPM).
        /// </remarks>
        /// <returns>List of detected beats.</returns>
        public List<Beat> DetectBeats(string wavePath, int minBpm, int maxBpm)
        {
            if (maxBpm > 2 * minBpm)
                throw new ArgumentOutOfRangeException("Provided BPM range is invalid. Max BPM can be at most 2 times min BPM.");

            using var reader = new WaveSampleReader(wavePath);

            if (reader.Channels != 2)
                throw new NotSupportedException("Current implementation requires stereo WAV file.");

            var detector = new BeatDetection.BeatDetectionV2(reader.SampleRate, minBpm, maxBpm);

            Span<float> buffer = stackalloc float[detector.WindowSize];
            while (reader.Read(buffer) == detector.WindowSize)
            {
                detector.DetectStep(buffer);
            }

            return detector.GetBeats().Select(b => (Beat)b).ToList();
        }

        /// <summary>
        /// Gets the global <see cref="SongLibrary"/> instance or if it doesn't exist returns an empty one.
        /// </summary>
        /// <returns>Global song library.</returns>
        public SongLibrary GetSongLibrary()
        {
            if (Content.Exists(SongLibraryUrl))
            {
                return Content.Get<SongLibrary>(SongLibraryUrl) ?? Content.Load<SongLibrary>(SongLibraryUrl);
            }
            else
            {
                return new SongLibrary()
                {
                    songs = new Dictionary<Guid, Song>(),
                    Attempts = new List<SongAttempt>(),
                };
            }
        }

        /// <summary>
        /// Creates a new <see cref="Sound"/> object and compresses the WAV data using the Celt encoder.
        /// </summary>
        /// <param name="wavePath">Path to the WAV file.</param>
        /// <param name="compressionBitRate">Target compression bitrate.</param>
        /// <param name="songId">Id of the song.</param>
        /// <returns>New (persisted to disk) <see cref="Sound"/> instance.</returns>
        public Sound WriteSoundData(string wavePath, BitRate compressionBitRate, Guid songId)
        {
            using var reader = new WaveSampleReader(wavePath);

            // This method is copied with modifications from Stride's SoundAssetCompiler.cs under MIT license
            using var outputStream = Content.FileProvider.OpenStream(SoundDataUrl(songId), VirtualFileMode.Create, VirtualFileAccess.Write, VirtualFileShare.Read, StreamFlags.Seekable);
            using var writer = new BinaryWriter(outputStream);
            
            var encoder = new Celt(reader.SampleRate, CompressedSoundSource.SamplesPerFrame, reader.Channels, false);

            var uncompressed = CompressedSoundSource.SamplesPerFrame * reader.Channels * sizeof(short);
            var target = (int)Math.Floor(uncompressed / compressionBitRate.CompressionRatio(reader.SampleRate));

            var newSound = new Sound
            {
                CompressedDataUrl = SoundDataUrl(songId),
                Channels = reader.Channels,
                SampleRate = reader.SampleRate,
                StreamFromDisk = true, // can be overriden when creating the sound instance
                Spatialized = false, // Spatialized would require mono (which we currently don't support for beat detection)
            };

            var delay = encoder.GetDecoderSampleDelay();

            var frameSize = CompressedSoundSource.SamplesPerFrame * reader.Channels;

            var outputBuffer = new byte[target];
            var buffer = new float[frameSize];
            var padding = reader.Channels * delay;
            var length = reader.SampleCount;
            for (var position = 0; position < length + padding; position += frameSize)
            {
                var read = reader.Read(buffer);

                // we didn't read the whole buffer length (no more data), so set the rest of it to 0.
                if (read < buffer.Length)
                {
                    Array.Clear(buffer, read, buffer.Length - read);
                }

                var len = encoder.Encode(buffer, outputBuffer);
                writer.Write((short)len);
                outputStream.Write(outputBuffer, 0, len);

                newSound.Samples += frameSize / reader.Channels;
                newSound.NumberOfPackets++;
                newSound.MaxPacketLength = Math.Max(newSound.MaxPacketLength, len);
            }

            // Samples is the real sound sample count, remove the delay at the end
            newSound.Samples -= delay;

            Content.Save(SoundUrl(songId), newSound);

            return newSound;
        }

        /// <summary>
        /// Creates a temporary <see cref="Sound"/> instance with a preloaded buffer with data from the WAV file.
        /// </summary>
        /// <param name="wavePath">Path to the WAV file.</param>
        /// <remarks>
        /// The buffer should be freed using <see cref="AudioLayer.BufferDestroy"/> when no longer needed.
        /// </remarks>
        /// <returns>New (temporary) <see cref="Sound"/> instance.</returns>
        public Sound InMemorySound(string wavePath)
        {
            using var reader = new WaveSampleReader(wavePath);

            var sound = new Sound
            {
                Channels = reader.Channels,
                SampleRate = reader.SampleRate,
                Samples = reader.SampleCount / reader.Channels,
                StreamFromDisk = false,
                Spatialized = false, // Spatialized would require mono (which we currently don't support for beat detection)
            };

            using var memory = new UnmanagedArray<short>(reader.SampleCount);

            int frameSize = 256 * reader.Channels;
            Span<float> readFrame = stackalloc float[frameSize];
            var writeFrame = new short[frameSize];

            int offset = 0;
            int read = 0;
            while ((read = reader.Read(readFrame)) != 0)
            {
                for (int i = 0; i < read; i++)
                    writeFrame[i] = (short)(readFrame[i] * 32768f);

                memory.Write(writeFrame, offset, 0, read);
                offset += read * sizeof(short);
            }

            sound.PreloadedBuffer = AudioLayer.BufferCreate(reader.SampleCount * sizeof(short));
            AudioLayer.BufferFill(sound.PreloadedBuffer, memory.Pointer, reader.SampleCount * sizeof(short), reader.SampleRate, reader.Channels == 1);

            return sound;
        }

        internal static string SongUrl(Guid id) => Invariant($"Songs/{id}");
        internal static string SoundUrl(Guid id) => Invariant($"Songs/{id}_Sound");
        internal static string SoundDataUrl(Guid id) => Invariant($"Songs/{id}_SoundData");
    }
}
