using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using RiseOfTheUndeaf.Core.Configuration;
using RiseOfTheUndeaf.Core.Intrinsics;
using Stride.Engine;

namespace RiseOfTheUndeaf.Core.Logging
{
    /// <summary>
    /// Extension methods to allow easier logging without the need to declare a logger in each class.
    /// </summary>
    public static class LogExtensions
    {
        private static ILoggerFactory loggerFactory;

        public static void InitializeLogging(this Game game)
        {
            loggerFactory = LoggerFactory.Create(builder => builder
                .SetMinimumLevel(UserSettings.Logging.MinLogLevel)
                .AddProvider(new StrideLoggerProvider()));
            game.Services.AddService(loggerFactory);
        }

        public static ILogger LogRaw(this object @this) => loggerFactory.CreateLogger(@this.GetType().FullName);

        #region Debug

        public static void LogDebug(this object @this, string message, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug(CreateEventId(typeName, memberName, sourceLineNumber), message);
        }

        public static void LogDebug<T0>(this object @this, string message, T0 arg0, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0);
        }

        public static void LogDebug<T0, T1>(this object @this, string message, T0 arg0, T1 arg1, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1);
        }

        public static void LogDebug<T0, T1, T2>(this object @this, string message, T0 arg0, T1 arg1, T2 arg2, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1, arg2);
        }

        public static void LogDebug<T0, T1, T2, T3>(this object @this, string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1, arg2, arg3);
        }
        #endregion Debug
        #region Info

        public static void LogInfo(this object @this, string message, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation(CreateEventId(typeName, memberName, sourceLineNumber), message);
        }

        public static void LogInfo<T0>(this object @this, string message, T0 arg0, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0);
        }

        public static void LogInfo<T0, T1>(this object @this, string message, T0 arg0, T1 arg1, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1);
        }

        public static void LogInfo<T0, T1, T2>(this object @this, string message, T0 arg0, T1 arg1, T2 arg2, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1, arg2);
        }

        public static void LogInfo<T0, T1, T2, T3>(this object @this, string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1, arg2, arg3);
        }
        #endregion Info
        #region Warning

        public static void LogWarning(this object @this, string message, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), message);
        }

        public static void LogWarning<T0>(this object @this, string message, T0 arg0, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0);
        }

        public static void LogWarning<T0, T1>(this object @this, string message, T0 arg0, T1 arg1, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1);
        }

        public static void LogWarning<T0, T1, T2>(this object @this, string message, T0 arg0, T1 arg1, T2 arg2, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1, arg2);
        }

        public static void LogWarning<T0, T1, T2, T3>(this object @this, string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1, arg2, arg3);
        }

        public static void LogWarning(this object @this, Exception exception, string message, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), exception, message);
        }

        public static void LogWarning<T0>(this object @this, Exception exception, string message, T0 arg0, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), exception, message, arg0);
        }

        public static void LogWarning<T0, T1>(this object @this, Exception exception, string message, T0 arg0, T1 arg1, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), exception, message, arg0, arg1);
        }

        public static void LogWarning<T0, T1, T2>(this object @this, Exception exception, string message, T0 arg0, T1 arg1, T2 arg2, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), exception, message, arg0, arg1, arg2);
        }

        public static void LogWarning<T0, T1, T2, T3>(this object @this, Exception exception, string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Warning)) logger.LogWarning(CreateEventId(typeName, memberName, sourceLineNumber), exception, message, arg0, arg1, arg2, arg3);
        }
        #endregion Warning
        #region Error

        public static void LogError(this object @this, string message, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), message);
        }

        public static void LogError<T0>(this object @this, string message, T0 arg0, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0);
        }

        public static void LogError<T0, T1>(this object @this, string message, T0 arg0, T1 arg1, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1);
        }

        public static void LogError<T0, T1, T2>(this object @this, string message, T0 arg0, T1 arg1, T2 arg2, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1, arg2);
        }

        public static void LogError<T0, T1, T2, T3>(this object @this, string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), message, arg0, arg1, arg2, arg3);
        }


        public static void LogError(this object @this, Exception exception, string message, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), exception, message);
        }

        public static void LogError<T0>(this object @this, Exception exception, string message, T0 arg0, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), exception, message, arg0);
        }

        public static void LogError<T0, T1>(this object @this, Exception exception, string message, T0 arg0, T1 arg1, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), exception, message, arg0, arg1);
        }

        public static void LogError<T0, T1, T2>(this object @this, Exception exception, string message, T0 arg0, T1 arg1, T2 arg2, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), exception, message, arg0, arg1, arg2);
        }

        public static void LogError<T0, T1, T2, T3>(this object @this, Exception exception, string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3, Separator sep = default, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var typeName = @this.GetType().FullName;
            var logger = loggerFactory.CreateLogger(typeName);
            if (logger.IsEnabled(LogLevel.Error)) logger.LogError(CreateEventId(typeName, memberName, sourceLineNumber), exception, message, arg0, arg1, arg2, arg3);
        }
        #endregion Error

        public static EventId CreateEventId(string fullTypeName, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EventId((int)CRC32.Compute(string.Concat(fullTypeName, memberName)) + sourceLineNumber, memberName);
        }

        public struct Separator { }
    }
}
