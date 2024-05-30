using osu.Framework;
using osu.Framework.Platform;

namespace fluXis.Game.Tests;

public static class Program
{
    public static void Main()
    {
        using GameHost host = Host.GetSuitableDesktopHost("fluXis-dev");
        using var game = new FluXisTestBrowser();
        host.Run(game);
    }
}
