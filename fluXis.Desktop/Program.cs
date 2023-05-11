using osu.Framework.Platform;
using osu.Framework;

namespace fluXis.Desktop;

public static class Program
{
    public static void Main()
    {
        using GameHost host = Host.GetSuitableDesktopHost(@"fluXis");
        using osu.Framework.Game game = new FluXisGameDesktop();
        host.Run(game);
    }
}
