using System;
using System.IO;
using fluXis.Utils;
using osu.Framework.IO.Network;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Utils;

namespace fluXis.UI.Tips;

public static class LoadingTips
{
    private const string tip_file = "tips.json";
    private const string online_path = "https://assets.flux.moe/tips.json";

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
            if (!storage.Exists(tip_file))
                return;

            var stream = storage.GetStream(tip_file);
            using var sr = new StreamReader(stream);
            tips = sr.ReadToEnd().Deserialize<string[]>();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to load tips from local storage!", LoggingTarget.Network);
        }
    }

    private static async void loadFromWeb(Storage storage)
    {
        try
        {
            var req = new WebRequest(online_path);
            await req.PerformAsync();
            var json = req.GetResponseString();
            tips = json.Deserialize<string[]>();

            var path = storage.GetFullPath(tip_file);
            await File.WriteAllTextAsync(path, json);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to download tips from web!", LoggingTarget.Network);
        }
    }
}
