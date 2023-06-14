using System.IO;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Menu;

public static class MenuSplashes
{
    public static string[] Splashes { get; private set; } =
    {
        "Literally 1984.",
        "What do you mean the chart is unreadable?",
        "*metal pipe sfx*",
        "Hey you wanna make a guest difficulty for my map?",
        "It's not a bug, it's a feature!",
        "This is made with the sole intention to create farm. It has no intresting non generic patterning and adds nothing to the ranked section but farm. This does not deserve to be ranked."
    };

    public static string RandomSplash => Splashes[RNG.Next(0, Splashes.Length)];

    public static void Load(Storage storage)
    {
        loadFromLocal(storage);
        loadFromWeb(storage);
    }

    private static void loadFromLocal(Storage storage)
    {
        try
        {
            if (!storage.Exists("splashes.json")) return;

            Logger.Log("Loading splashes from local storage");

            var stream = storage.GetStream("splashes.json");
            using var sr = new StreamReader(stream);
            var json = sr.ReadToEnd();
            Splashes = JsonConvert.DeserializeObject<string[]>(json);

            Logger.Log("Splashes loaded from local storage");
        }
        catch
        {
            Logger.Log("Failed to load splashes from local storage");
        }
    }

    private static async void loadFromWeb(Storage storage)
    {
        try
        {
            Logger.Log("Downloading splashes from web", LoggingTarget.Network);
            var req = new WebRequest("https://fluxis.foxes4life.net/splashes.json");
            await req.PerformAsync();
            var json = req.GetResponseString();
            Splashes = JsonConvert.DeserializeObject<string[]>(json);

            Logger.Log("Saving splashes to local storage", LoggingTarget.Network);

            var path = storage.GetFullPath("splashes.json");
            await File.WriteAllTextAsync(path, json);

            Logger.Log("Splashes saved to local storage", LoggingTarget.Network);
        }
        catch
        {
            Logger.Log("Failed to download splashes from web", LoggingTarget.Network);
        }
    }
}
