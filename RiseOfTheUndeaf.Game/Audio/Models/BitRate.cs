using System;

namespace RiseOfTheUndeaf.Audio.Models
{
    /// <summary>
    /// Audio bit rate preset for compression.
    /// </summary>
    public enum BitRate
    {
        Kbps320,
        Kbps256,
        Kbps128
    }

    public static class BitRateExtensions
    {
        public static double CompressionRatio(this BitRate bitRate, int sampleRate) =>
            bitRate switch
            {
                // 128 - 1024 bits in bytes
                BitRate.Kbps320 => 1.0 * sampleRate * sizeof(short) / (320 * 128),
                BitRate.Kbps256 => 1.0 * sampleRate * sizeof(short) / (256 * 128),
                BitRate.Kbps128 => 1.0 * sampleRate * sizeof(short) / (128 * 128),
                _ => throw new NotSupportedException(),
            };
    }
}
