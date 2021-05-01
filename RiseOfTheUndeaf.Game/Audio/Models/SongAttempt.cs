using System;

namespace RiseOfTheUndeaf.Audio.Models
{
    /// <summary>
    /// A summary of a song attempt.
    /// </summary>
    public struct SongAttempt
    {
        /// <summary>
        /// Song that has been played.
        /// </summary>
        public Song Song;

        /// <summary>
        /// When was the attempt made.
        /// </summary>
        public DateTime AttemptTime;

        /// <summary>
        /// How long player played before the song ended or they failed.
        /// </summary>
        public TimeSpan AttemptLength;

        /// <summary>
        /// Number of points awarded.
        /// </summary>
        public long Score;

        /// <summary>
        /// Wether the attempt was failed (player died).
        /// </summary>
        public bool Failed { get; set; }

        /// <summary>
        /// Number of zombies killed by player.
        /// </summary>
        public int ZombiesKilled;
    }
}