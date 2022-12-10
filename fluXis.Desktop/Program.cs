using osu.Framework.Platform;
using osu.Framework;
using fluXis.Game;

namespace fluXis.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"fluXis"))
            using (osu.Framework.Game game = new FluXisGame())
                host.Run(game);
        }
    }
}
