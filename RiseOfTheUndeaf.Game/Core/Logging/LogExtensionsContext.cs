using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;

namespace RiseOfTheUndeaf.Core.Logging
{
    internal static class LogExtensionsContext
    {
        public static void InitializeContext(Game game)
        {
            var globalContext = new Dictionary<string, object>()
            {
                { "sessionId", Guid.NewGuid() },
                { "windowManager", game.Window.GetType().Name },
                { "graphicsPlatform", GraphicsDevice.Platform },
                { "processArch", RuntimeInformation.ProcessArchitecture },
                { "version", GetVersion() },
                { "strideVersion", GetStrideVersion() },
                { "OSVersion", Environment.OSVersion },
                { "runtimeVersion", Environment.Version },
            };

            var scope = LogContextProvider.CreateScope(globalContext);
            game.Exiting += (sender, args) => scope.Dispose();
        }

        private static string GetVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var fullVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var hash = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(attr => attr.Key == "GitHash")
                ?.Value;

            if (hash == null)
            {
                return fullVersionAttr.InformationalVersion;
            }
            else
            {
                return $"{fullVersionAttr.InformationalVersion}-{hash}";
            }
        }
        private static string GetStrideVersion()
        {
            var strideAssembly = typeof(GameBase).Assembly;
            var strideVersion = strideAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            return strideVersion;
        }
    }
}
