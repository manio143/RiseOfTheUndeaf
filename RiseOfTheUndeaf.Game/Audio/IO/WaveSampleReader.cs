using System;
using System.Buffers;
using System.IO;
using NAudio.Wave;
using Stride.Core.IO;

namespace RiseOfTheUndeaf.Audio.IO
{
    /// <summary>
    /// Reader of wave samples. Wraps over <see cref="NAudio.Wave.WaveFileReader"/>.
    /// </summary>
    public sealed class WaveSampleReader : IDisposable
    {
        private Stream fileStream;
        private WaveFileReader waveFileReader;

        public int Channels => waveFileReader.WaveFormat.Channels;

        public int SampleRate => waveFileReader.WaveFormat.SampleRate;

        public int SampleCount => (int)waveFileReader.SampleCount * Channels;

        public WaveSampleReader(string filePath)
        {
            fileStream = File.Open(filePath, FileMode.Open);
            waveFileReader = new WaveFileReader(fileStream);
        }

        public WaveSampleReader(IVirtualFileProvider fileProvider, string url)
        {
            fileStream = fileProvider.OpenStream(url, VirtualFileMode.Open, VirtualFileAccess.Read);
            waveFileReader = new WaveFileReader(fileStream);
        }

        public WaveSampleReader(Stream stream)
        {
            waveFileReader = new WaveFileReader(stream);
        }

        public void Dispose()
        {
            waveFileReader?.Dispose();
            fileStream?.Dispose();
        }

        /// <summary>
        /// Reads samples from the WAVE file.
        /// </summary>
        /// <param name="output">Buffer to write to, length has to be a multiplication of <see cref="Channels"/>.</param>
        /// <returns>Number of samples written to the output.</returns>
        public int Read(Span<float> output)
        {
            if (output.Length % Channels != 0)
                throw new ArgumentException("Output span has to have a length that is a multiplication of number of channels.");

            // This method was copied with slight modifications from NAudio under MIT license.
            switch (waveFileReader.WaveFormat.Encoding)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.IeeeFloat:
                case WaveFormatEncoding.Extensible: // n.b. not necessarily PCM, should probably write more code to handle this case
                    break;
                default:
                    throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
            }

            int bitsPerSample = waveFileReader.WaveFormat.BitsPerSample;
            int bytesToRead = output.Length * (bitsPerSample / 8);
            byte[] readBuffer = ArrayPool<byte>.Shared.Rent(bytesToRead);

            try
            {
                int bytesRead = waveFileReader.Read(readBuffer, 0, bytesToRead);
                if (bytesRead == 0) return 0; // end of file
                if (bytesRead < bytesToRead && bytesRead % Channels != 0)
                    throw new InvalidDataException("Unexpected end of file");

                var samplesRead = bytesRead / (bitsPerSample / 8);

                int offset = 0;
                for (int sample = 0; sample < samplesRead; sample++)
                {
                    if (bitsPerSample == 16)
                    {
                        output[sample] = BitConverter.ToInt16(readBuffer, offset) / 32768f;
                        offset += 2;
                    }
                    else if (bitsPerSample == 24)
                    {
                        output[sample] = (((sbyte)readBuffer[offset + 2] << 16) | (readBuffer[offset + 1] << 8) | readBuffer[offset]) / 8388608f;
                        offset += 3;
                    }
                    else if (bitsPerSample == 32 && waveFileReader.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                    {
                        output[sample] = BitConverter.ToSingle(readBuffer, offset);
                        offset += 4;
                    }
                    else if (bitsPerSample == 32)
                    {
                        output[sample] = BitConverter.ToInt32(readBuffer, offset) / (Int32.MaxValue + 1f);
                        offset += 4;
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported bit depth");
                    }
                }

                return samplesRead;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(readBuffer);
            }
        }
    }
}
