using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using RiseOfTheUndeaf.Core.Configuration;

namespace RiseOfTheUndeaf.Core.Logging
{
    internal class StrideLoggerProvider : ILoggerProvider
    {
        static StrideLoggerProvider()
        {
            // We're setting our own log level, so setting global mininum to the lowest.
            Stride.Core.Diagnostics.GlobalLogger.MinimumLevelEnabled = Stride.Core.Diagnostics.LogMessageType.Debug;
        }

        public ILogger CreateLogger(string categoryName)
        {
            // The logger is cached by the LoggerFactory
            return new StrideLogger(Stride.Core.Diagnostics.GlobalLogger.GetLogger(categoryName));
        }

        public void Dispose()
        {
        }

        private class StrideLogger : ILogger
        {
            private Stride.Core.Diagnostics.ILogger logger;

            public StrideLogger(Stride.Core.Diagnostics.ILogger logger) =>
                this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotSupportedException("Logging scopes are currently not supported."); // Do I want to support this? Could be useful for AsyncScript scenarios
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= UserSettings.Logging.MinLogLevel;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (logLevel == LogLevel.None) return;

                logger.Log(new Stride.Core.Diagnostics.SerializableLogMessage(
                    logger.Module.Split('.').Last(),
                    ConvertLevelToType(logLevel),
                    formatter(state, exception),
                    exception != null ? new Stride.Core.Diagnostics.ExceptionInfo(exception) : null));
            }

            private Stride.Core.Diagnostics.LogMessageType ConvertLevelToType(LogLevel logLevel)
            {
                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        return Stride.Core.Diagnostics.LogMessageType.Debug;
                    case LogLevel.Information:
                        return Stride.Core.Diagnostics.LogMessageType.Info;
                    case LogLevel.Warning:
                        return Stride.Core.Diagnostics.LogMessageType.Warning;
                    case LogLevel.Error:
                        return Stride.Core.Diagnostics.LogMessageType.Error;
                    case LogLevel.Critical:
                        return Stride.Core.Diagnostics.LogMessageType.Fatal;
                    default:
                        throw new NotSupportedException($"Unsupported log level '{logLevel}'.");
                }
            }
        }
    }
}
