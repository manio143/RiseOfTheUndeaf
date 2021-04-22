using Microsoft.Extensions.Logging;

namespace RiseOfTheUndeaf.Core.Configuration
{
    [ConfigurationSection("Logging")]
    public class LoggingConfiguration
    {
        public LogLevel MinLogLevel { get; set; } =
#if DEBUG
            LogLevel.Trace;
#else
            LogLevel.Warning;
#endif
    }
}
