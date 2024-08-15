using System.Linq;
using osu.Framework;
using osu.Framework.Platform;

namespace fluXis.Game.Tests;

public static class Program
{
    public static void Main(string[] args)
    {
        var name = args.Contains("--stable") ? "fluXis" : "fluXis-dev";

        using GameHost host = Host.GetSuitableDesktopHost(name);
        using var game = new FluXisTestBrowser();
        host.Run(game);
    }
}
