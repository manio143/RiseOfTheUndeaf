using System;
using System.Collections.Generic;
using Stride.Audio;
using Stride.Core;
using Stride.Core.Serialization.Contents;

namespace RiseOfTheUndeaf.Audio.Models
{
    /// <summary>
    /// A song metadata linking beats to audio.
    /// </summary>
    [DataContract]
    [ContentSerializer(typeof(DataContentSerializer<Song>))]
    public class Song
    {
        /// <summary>
        /// Id in the system.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the song.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sound object from which you can play audio.
        /// </summary>
        public Sound Sound { get; set; }

        /// <summary>
        /// Collection of beats for the song.
        /// </summary>
        public List<Beat> Beats { get; set; }

        /// <summary>
        /// How long is the song.
        /// </summary>
        public TimeSpan Length => Sound.TotalLength;
    }
}
