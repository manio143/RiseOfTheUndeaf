using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RiseOfTheUndeaf.Audio.Models;
using RiseOfTheUndeaf.Core.Logging;
using Stride.Audio;
using Stride.Core;
using Stride.Core.IO;
using Stride.Engine;
using static System.FormattableString;

namespace RiseOfTheUndeaf.Audio.Components
{
    public class CreateSongScript : StartupScript
    {
        public const string SongLibraryUrl = "SongLibrary";

        [Display("WAV file location")]
        public string WAVPath { get; set; }
        
        public string Name { get; set; }

        public int MinBpm { get; set; }
        public int MaxBpm { get; set; }

        public BitRate BitRate { get; set; }

        public override void Start()
        {
            if (MaxBpm > 2 * MinBpm)
            {
                this.LogError("Provided BPM range is invalid. Max BPM can be at most 2 times min BPM.");
                return;
            }

            try
            {
                SongLibrary library = GetSongLibrary();

                if (library.Songs.Any(song => song.Name == Name))
                {
                    this.LogWarning("Song named '{songName}' already exists.", Name);
                    return;
                }

                Song song = CreateSong();
                
                library.songs.Add(song.Id, song);
                Content.Save(SongLibraryUrl, library);
            }
            catch(Exception ex)
            {
                this.LogError(ex, "An error occurred when creating song '{songName}'.", Name);
                throw;
            }
        }

        /// <summary>
        /// This is a huge method (but it has to be in order to properly do streaming).
        /// I'm opening the input wavefile using NAudio (so it takes any WAVE file).
        /// I'm opening an output stream for compressed data (replicating what SoundAssetCompiler does).
        /// I'm creating the beat detector.
        /// Then in a loop:
        ///  - read 512 samples from WAV
        ///  - feed them to the beat detector
        ///  - compress it with Celt push to output stream
        /// Finally create a Sound and Song objects and save them to disk.
        /// </summary>
        /// <returns></returns>
        private Song CreateSong()
        {
            Song song = new Song();
            song.Name = Name;
            song.Id = Guid.NewGuid();

            var songUrl = Invariant($"Songs/{song.Id}");
            var soundUrl = Invariant($"{songUrl}_Sound");
            var soundDataUrl = Invariant($"{soundUrl}_Data");

            Sound sound = new Sound
            {
                Name = Invariant($"{Name} (Sound)"),
                CompressedDataUrl = soundDataUrl,
                StreamFromDisk = true,
                Spatialized = false, // I think there's an implication that spatialized = mono
            };
            song.Sound = sound;

            using (var file = File.Open(WAVPath, FileMode.Open))
            using (var waveReader = new NAudio.Wave.WaveFileReader(file))

            using (var outputStream = Content.FileProvider.OpenStream(soundDataUrl, VirtualFileMode.Create, VirtualFileAccess.Write, VirtualFileShare.Read, StreamFlags.Seekable))
            using (var writer = new BinaryWriter(outputStream))
            {
                var channels = sound.Channels = waveReader.WaveFormat.Channels;
                var sampleRate = sound.SampleRate = waveReader.WaveFormat.SampleRate;

                if (channels != 2)
                    throw new NotSupportedException("Current implementation requires stereo WAV file.");

                var detection = new BeatDetection.BeatDetectionV2(sampleRate, MinBpm, MaxBpm);

                const int compressionFrameSize = CompressedSoundSource.SamplesPerFrame * 2; // 512 * 2 channels

                detection.WindowSize = compressionFrameSize;

                var targetPacketSize = (int)Math.Floor(compressionFrameSize * sizeof(short) / BitRate.CompressionRatio(sampleRate));

                var encoder = new Celt(sampleRate, compressionFrameSize / 2, channels, false);
                var delay = encoder.GetDecoderSampleDelay();

                var outputBuffer = new byte[targetPacketSize];
                var buffer = new float[compressionFrameSize];
                var count = 0;
                var padding = sizeof(float) * channels * delay;

                float[] readFrame = null;
                while ((readFrame = waveReader.ReadNextSampleFrame()) != null)
                {
                    if (count == compressionFrameSize) //flush
                    {
                        detection.DetectStep(buffer); // perform beat detection

                        var len = encoder.Encode(buffer, outputBuffer);
                        writer.Write((short)len);
                        outputStream.Write(outputBuffer, 0, len);

                        sound.Samples += count / channels;
                        sound.NumberOfPackets++;
                        sound.MaxPacketLength = Math.Max(sound.MaxPacketLength, len);

                        count = 0;
                        Array.Clear(buffer, 0, compressionFrameSize);
                    }

                    foreach (var channel in readFrame)
                    {
                        buffer[count++] = channel;
                    }
                }

                // Pad with 0 once we reach end of stream (this is needed because of encoding delay)
                for (int i = 1; i <= padding; i++)
                {
                    if (count == compressionFrameSize || (i == padding && count > 0)) //flush
                    {
                        // We're skipping the beat detection for the final packets as it's unlikely to matter.

                        var len = encoder.Encode(buffer, outputBuffer);
                        writer.Write((short)len);
                        outputStream.Write(outputBuffer, 0, len);

                        sound.Samples += count / channels;
                        sound.NumberOfPackets++;
                        sound.MaxPacketLength = Math.Max(sound.MaxPacketLength, len);

                        count = 0;
                    }

                    buffer[count++] = 0;
                }

                // Samples is the real sound sample count, remove the delay at the end
                sound.Samples -= delay;

                song.Beats = detection.GetBeats().Select(b => (Beat)b).ToList();
            }

            Content.Save(soundUrl, sound);
            Content.Save(songUrl, song);

            return song;
        }

        private SongLibrary GetSongLibrary()
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
    }
}
