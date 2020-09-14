using Stride.Engine;

namespace RiseOfTheUndeaf
{
    class RiseOfTheUndeafApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.ConsoleLogMode = Stride.Core.Diagnostics.ConsoleLogMode.None;
                game.Run();
            }
        }
    }
}
