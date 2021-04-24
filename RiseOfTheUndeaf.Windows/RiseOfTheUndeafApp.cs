using RiseOfTheUndeaf.Core.Configuration;
using RiseOfTheUndeaf.Core.Logging;
using Stride.Core.Diagnostics;
using Stride.Engine;
using System.Diagnostics;

namespace RiseOfTheUndeaf
{
    class RiseOfTheUndeafApp
    {
        static void Main(string[] args)
        {
            using (var game = new CustomGame())
            {
                game.InitializeLogging();
                game.Run();
                UserSettings.SaveSettings();
            }
        }
    }

    class CustomGame : Game
    {
        protected override LogListener GetLogListener()
        {
            return new DebugLogListener();
        }
    }

    class DebugLogListener : LogListener
    {
        protected override void OnLog(ILogMessage logMessage)
        {
            var exceptionMsg = GetExceptionText(logMessage);

            if (Debugger.IsAttached)
            {
                // Log the actual message
                Debug.WriteLine(GetDefaultText(logMessage));
                if (!string.IsNullOrEmpty(exceptionMsg))
                {
                    Debug.WriteLine(exceptionMsg);
                }
            }
        }
    }
}
