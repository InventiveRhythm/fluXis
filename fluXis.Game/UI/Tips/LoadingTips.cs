using System.IO;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Utils;

namespace fluXis.Game.UI.Tips;

public static class LoadingTips
{
    private static string[] tips = { "super awesome tip" };

    public static string RandomTip => tips[RNG.Next(0, tips.Length)];

    public static void Load(Storage storage)
    {
        loadFromLocal(storage);
        loadFromWeb(storage);
    }

    private static void loadFromLocal(Storage storage)
    {
        try
        {
            if (!storage.Exists("tips.json")) return;

            Logger.Log("Loading tips from local storage", LoggingTarget.Runtime, LogLevel.Debug);

            var stream = storage.GetStream("tips.json");
            using var sr = new StreamReader(stream);
            var json = sr.ReadToEnd();
            tips = JsonConvert.DeserializeObject<string[]>(json);

            Logger.Log("Tips loaded from local storage", LoggingTarget.Runtime, LogLevel.Debug);
        }
        catch
        {
            Logger.Log("Failed to load tips from local storage", LoggingTarget.Runtime, LogLevel.Error);
        }
    }

    private static async void loadFromWeb(Storage storage)
    {
        try
        {
            Logger.Log("Downloading tips from web...", LoggingTarget.Network, LogLevel.Debug);
            var req = new WebRequest("https://fluxis.foxes4life.net/tips.json");
            await req.PerformAsync();
            var json = req.GetResponseString();
            tips = JsonConvert.DeserializeObject<string[]>(json);

            Logger.Log("Saving tips to local storage...", LoggingTarget.Network, LogLevel.Debug);

            var path = storage.GetFullPath("tips.json");
            await File.WriteAllTextAsync(path, json);

            Logger.Log("Tips saved to local storage", LoggingTarget.Network, LogLevel.Debug);
        }
        catch
        {
            Logger.Log("Failed to download tips from web", LoggingTarget.Network, LogLevel.Error);
        }
    }
}
