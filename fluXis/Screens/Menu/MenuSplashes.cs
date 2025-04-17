using System;
using System.IO;
using fluXis.Utils;
using osu.Framework.IO.Network;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Utils;

namespace fluXis.Screens.Menu;

public static class MenuSplashes
{
    private const string splash_file = "splashes.json";
    private const string online_path = "https://assets.flux.moe/splashes.json";

    private static string[] splashes =
    {
        "Literally 1984.",
        "We do a little bit of trolling",
        "The tiebreaker is undeniably the essence of all tournaments. It is the fruit of hard work, the culmination of a valiant effort to fight and the manifestation of absolute determination. It is the desperation that creates friendship, the flood of adrenaline that replenishes life and the myriad of emotions that make one feel ever so momentarily blissful, that they could relish in the very moment where everyone belongs in the game. I really love the tiebreaker.",
        "What do you mean the chart is unreadable?",
        "*metal pipe sfx*",
        "Hey you wanna make a guest difficulty for my map?",
        "It's not a bug, it's a feature!",
        "This is made with the sole intention to create farm. It has no intresting non generic patterning and adds nothing to the ranked section but farm. This does not deserve to be ranked."
    };

    public static string RandomSplash => splashes[RNG.Next(0, splashes.Length)];

    public static void Load(Storage storage)
    {
        loadFromLocal(storage);
        loadFromWeb(storage);
    }

    private static void loadFromLocal(Storage storage)
    {
        try
        {
            if (!storage.Exists(splash_file))
                return;

            var stream = storage.GetStream(splash_file);
            using var sr = new StreamReader(stream);
            splashes = sr.ReadToEnd().Deserialize<string[]>();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load splashes from local storage!");
        }
    }

    private static async void loadFromWeb(Storage storage)
    {
        try
        {
            var req = new WebRequest(online_path);
            await req.PerformAsync();
            var json = req.GetResponseString();
            splashes = json.Deserialize<string[]>();

            var path = storage.GetFullPath(splash_file);
            await File.WriteAllTextAsync(path, json);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to download splashes from web!", LoggingTarget.Network);
        }
    }
}
