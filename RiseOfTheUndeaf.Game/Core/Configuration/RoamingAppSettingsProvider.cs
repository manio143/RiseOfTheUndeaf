using Stride.Core.Settings;

namespace RiseOfTheUndeaf.Core.Configuration
{
    public class RoamingAppSettingsProvider : IAppSettingsProvider
    {
        public AppSettings LoadAppSettings()
        {
            // TODO implement an INI reader from
            // VirtualFileSystem.ApplicationRoaming. "config.ini"
            return new AppSettings(); 
        }

        public void WriteAppSettings(AppSettings appSettings)
        {
            // TODO
        }
    }
}
