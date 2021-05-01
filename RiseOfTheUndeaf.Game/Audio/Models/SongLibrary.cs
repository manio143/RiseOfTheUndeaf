using System;
using System.Collections.Generic;
using RiseOfTheUndeaf.Audio.Serialization;
using Stride.Core.Serialization;
using Stride.Core.Serialization.Contents;

namespace RiseOfTheUndeaf.Audio.Models
{
    /// <summary>
    /// Library of songs registered in the game.
    /// </summary>
    [DataSerializer(typeof(SongLibrarySerializer))]
    [ContentSerializer(typeof(DataContentSerializer<SongLibrary>))]
    public class SongLibrary
    {
        internal Dictionary<Guid, Song> songs;

        /// <summary>
        /// History of games played.
        /// </summary>
        public List<SongAttempt> Attempts { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<Song> Songs => songs.Values;
    }
}
