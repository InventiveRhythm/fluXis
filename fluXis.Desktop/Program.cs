using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game;
using fluXis.Game.IPC;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Utils;
using osu.Framework.Platform;
using osu.Framework;
using osu.Framework.Localisation;
using osu.Framework.Logging;

namespace fluXis.Desktop;

public static class Program
{
    public static string[] Args { get; private set; } = Array.Empty<string>();

    public static void Main(string[] args)
    {
        if (args.Contains("--generate-langfiles"))
        {
            generateDefaultLangfiles();
            return;
        }

        Args = args;

        string name = "fluXis";

        if (args.Contains("--profile"))
        {
            var idx = Array.IndexOf(args, "--profile");

            if (args.Length <= idx + 1)
                throw new ArgumentException("No profile name provided.");

            var profile = args[idx + 1].ToLower();

            name += $"-{profile}";
            Console.WriteLine($"Running with profile {profile}");
        }

        if (OperatingSystem.IsWindows())
            FileExtensionHelper.EnsureAssociationsSet();

        using GameHost host = Host.GetSuitableDesktopHost(name, new HostOptions { IPCPort = 44127 });

        switch (host.IsPrimaryInstance)
        {
            case false when sendIpcMessage(host, args):
                return;

            case false when !FluXisGameBase.IsDebug:
                Logger.Log("fluXis does not support multiple running instances.", LoggingTarget.Runtime, LogLevel.Error);
                return;

            case true:
            {
                var ipc = new TcpIpcProvider(24242);
                ipc.Bind();
                break;
            }
        }

        var game = new FluXisGameDesktop();
        host.Run(game);
    }

    private static bool sendIpcMessage(IIpcHost host, IReadOnlyList<string> args)
    {
        if (args.Count <= 0 || !args[0].Contains('.')) return false;

        foreach (string file in args)
        {
            var channel = new IPCImportChannel(host);

            if (!channel.Import(file).Wait(3000))
                throw new TimeoutException();

            return true;
        }

        return false;
    }

    private static void generateDefaultLangfiles()
    {
        // used to generate the default english langfiles

        var cats = new List<LocalizationCategory>
        {
            new SettingsAppearanceStrings(),
            new SettingsAudioStrings(),
            new SettingsDebugStrings(),
            new SettingsGameplayStrings(),
            new SettingsGeneralStrings(),
            new SettingsGraphicsStrings(),
            new SettingsInputStrings(),
            new SettingsMaintenanceStrings(),
            new SettingsPluginsStrings(),
            new SettingsUIStrings()
        };

        Directory.CreateDirectory("langfiles");

        foreach (var cat in cats)
        {
            var file = cat.FileName;
            var props = cat.GetType().GetProperties().Where(p => p.PropertyType == typeof(TranslatableString));

            var dict = new Dictionary<string, string>();

            foreach (var prop in props)
            {
                var value = (TranslatableString)prop.GetValue(cat)!;

                var key = value.Key.Split(':')[1];
                var defaultStr = value.Format;

                dict.Add(key, defaultStr);
            }

            var json = dict.Serialize(true);
            File.WriteAllText($"langfiles/{file}.json", json);
        }
    }
}
