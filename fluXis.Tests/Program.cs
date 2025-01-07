using osu.Framework;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Tests;

public static class Program
{
    private static bool useStableData;
    public static string TestSetID { get; private set; }
    public static string TestMapID { get; private set; }

    public static void Main(string[] args)
    {
        parseArgs(args);
        var name = useStableData ? "fluXis" : "fluXis-dev";

        Logger.Log($"Using stable data: {useStableData}.");
        Logger.Log($"Test MapSet ID: {TestSetID}.");
        Logger.Log($"Test Map ID: {TestMapID}.");

        using GameHost host = Host.GetSuitableDesktopHost(name);
        using var game = new FluXisTestBrowser();
        host.Run(game);
    }

    private static void parseArgs(string[] args)
    {
        var inSet = false;
        var inMap = false;

        foreach (var arg in args)
        {
            if (inSet)
            {
                TestSetID = arg;
                inSet = false;
                continue;
            }

            if (inMap)
            {
                TestMapID = arg;
                inMap = false;
                continue;
            }

            switch (arg)
            {
                case "--stable":
                    useStableData = true;
                    continue;

                case "--test-set":
                    inSet = true;
                    continue;

                case "--test-map":
                    inMap = true;
                    continue;
            }
        }
    }
}
