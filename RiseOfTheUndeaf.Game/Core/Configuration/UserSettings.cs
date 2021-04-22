using Stride.Core.Settings;

namespace RiseOfTheUndeaf.Core.Configuration
{
    public static class UserSettings
    {
        private static AppSettings settings = AppSettingsManager.Settings;

        /// <summary>
        /// Gets the settings value from AppSettings or creates a new instance.
        /// </summary>
        private static TSettings Get<TSettings>(this AppSettings appSettings)
            where TSettings : class, new()
            => appSettings.GetSettings<TSettings>() ?? new TSettings();

        public static LoggingConfiguration Logging { get; } = settings.Get<LoggingConfiguration>();
    }
}
