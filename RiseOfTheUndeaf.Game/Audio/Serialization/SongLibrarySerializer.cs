using System;
using System.Collections.Generic;
using System.Linq;
using RiseOfTheUndeaf.Audio.Models;
using RiseOfTheUndeaf.Core.Logging;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Serialization;
using Stride.Core.Serialization.Contents;

namespace RiseOfTheUndeaf.Audio.Serialization
{
    internal class SongLibrarySerializer : DataSerializer<SongLibrary>
    {
        private DataSerializer<Guid> guidSerializer;
        private DataSerializer<TimeSpan> timeSerializer;
        private DataSerializer<DateTime> dateSerializer;

        public override void Initialize([NotNull] SerializerSelector serializerSelector)
        {
            guidSerializer = serializerSelector.GetSerializer<Guid>();
            timeSerializer = serializerSelector.GetSerializer<TimeSpan>();
            dateSerializer = serializerSelector.GetSerializer<DateTime>();
        }

        public override void Serialize(ref SongLibrary obj, ArchiveMode mode, [NotNull] SerializationStream stream)
        {
            if (mode == ArchiveMode.Deserialize)
            {
                var services = stream.Context.Tags.Get(ServiceRegistry.ServiceRegistryKey);
                var contentManger = services.GetService<IContentManager>();

                // load songs
                {
                    int idCount = stream.ReadInt32();
                    obj.songs = new Dictionary<Guid, Song>(idCount);

                    for (int i = 0; i < idCount; i++)
                    {
                        Guid id = new Guid();
                        guidSerializer.Serialize(ref id, mode, stream);

                        var songUrl = FormattableString.Invariant($"Songs/{id}");
                        var song = contentManger.Get<Song>(songUrl) ?? contentManger.Load<Song>(songUrl);

                        if (song == null)
                        {
                            this.LogWarning("Song with id '{songId}' has not been found. Skipping.", id);
                            continue;
                        }

                        obj.songs.Add(id, song);
                    }
                }

                // load attempts
                {
                    int attemptCount = stream.ReadInt32();
                    obj.Attempts = new List<SongAttempt>(attemptCount);

                    for (int i = 0; i < attemptCount; i++)
                    {
                        var attempt = new SongAttempt();

                        Guid songId = new Guid();
                        guidSerializer.Serialize(ref songId, mode, stream);

                        if (!obj.songs.TryGetValue(songId, out Song song))
                        {
                            this.LogWarning("Song with id '{songId}' hasn't been loaded in the library. Removing entry from history.", songId);
                            song = null;
                        }

                        attempt.Song = song;

                        dateSerializer.Serialize(ref attempt.AttemptTime, mode, stream);
                        timeSerializer.Serialize(ref attempt.AttemptLength, mode, stream);
                        attempt.Score = stream.ReadInt64();
                        attempt.ZombiesKilled = stream.ReadInt32();
                        attempt.Failed = stream.ReadBoolean();

                        if (attempt.Song != null)
                            obj.Attempts.Add(attempt);
                    }
                }
            }
            else // serialize
            {
                var ids = obj.songs.Keys;
                // serialize the collection of songs - we just store their ids here, as each song has a URL
                {
                    stream.Write(ids.Count);
                    foreach (Guid id in ids)
                        guidSerializer.Serialize(id, stream);
                }

                // serialize attempts
                {
                    stream.Write(obj.Attempts.Count);
                    foreach (var attempt in obj.Attempts)
                    {
                        guidSerializer.Serialize(obj.songs.First(kvp => kvp.Value == attempt.Song).Key, stream);
                        dateSerializer.Serialize(attempt.AttemptTime, stream);
                        timeSerializer.Serialize(attempt.AttemptLength, stream);
                        stream.Write(attempt.Score);
                        stream.Write(attempt.ZombiesKilled);
                        stream.Write(attempt.Failed);
                    }
                }
            }
        }
    }
}
