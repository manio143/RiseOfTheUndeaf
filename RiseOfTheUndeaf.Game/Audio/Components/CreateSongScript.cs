using RiseOfTheUndeaf.Audio.IO;
using RiseOfTheUndeaf.Audio.Models;
using RiseOfTheUndeaf.Core.Logging;
using Stride.Core;
using Stride.Engine;

namespace RiseOfTheUndeaf.Audio.Components
{
    public class CreateSongScript : StartupScript
    {
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

            new SongContentProvider(Content).AddSongToLibrary(Name, WAVPath, BitRate, MinBpm, MaxBpm);
        }
    }
}
