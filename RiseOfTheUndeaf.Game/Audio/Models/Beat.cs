using System;
using Stride.Core;

namespace RiseOfTheUndeaf.Audio.Models
{
    /// <summary>
    /// A single beat in a song.
    /// </summary>
    [DataContract]
    public struct Beat
    {
        public TimeSpan TimeOffset { get; set; }
        public BeatType Type { get; set; }

        public static implicit operator Beat(BeatDetection.DetectedBeat detectedBeat) =>
            new Beat()
            {
                TimeOffset = detectedBeat.TimeOffset,
                Type = detectedBeat.StrongestFrequency < 0.45 ? BeatType.Lower : BeatType.Higher,
            };
    }

    public enum BeatType : byte
    {
        /// <summary>
        /// Lower beat (based on frequency), e.g. a drum.
        /// </summary>
        Lower,

        /// <summary>
        /// Higher beat (based on frequency), e.g. a snare.
        /// </summary>
        Higher,
    }
}
