﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using fluXis.IPC;
using fluXis.Localization;
using fluXis.Localization.Categories;
using fluXis.Localization.Categories.Settings;
using fluXis.Mods;
using fluXis.Utils;
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

#if CLOSED_TESTING
        name += "-ct";
#endif

        if (args.Contains("--profile"))
        {
            var idx = Array.IndexOf(args, "--profile");

            if (args.Length <= idx + 1)
                throw new ArgumentException("No profile name provided.");

            var profile = args[idx + 1].ToLower();

            name += $"-{profile}";
            Console.WriteLine($"Running with profile {profile}");
        }

        if (Args.Contains("--post-update"))
        {
            try
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "patches");

                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
            }
            catch { }
        }

        if (OperatingSystem.IsWindows())
            FileExtensionHelper.EnsureAssociationsSet();

        using GameHost host = Host.GetSuitableDesktopHost(name, new HostOptions { IPCPipeName = name });

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
            new DashboardStrings(),
            new GeneralStrings(),
            new MainMenuStrings(),
            new ModSelectStrings(),
            new ModStrings(),
            new SettingsAppearanceStrings(),
            new SettingsAudioStrings(),
            new SettingsDebugStrings(),
            new SettingsGameplayStrings(),
            new SettingsGeneralStrings(),
            new SettingsGraphicsStrings(),
            new SettingsInputStrings(),
            new SettingsMaintenanceStrings(),
            new SettingsPluginsStrings(),
            new SettingsUIStrings(),
            new SongSelectStrings(),
            new ToolbarStrings(),
        };

        Directory.CreateDirectory("langfiles");

        var count = 0;
        var words = 0;

        foreach (var cat in cats)
        {
            var file = cat.FileName;
            var props = cat.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                           .Where(p => p.PropertyType == typeof(TranslatableString) || p.PropertyType == typeof(LocalisableString));

            var dict = new Dictionary<string, string>();

            if (cat is ModStrings modstr)
            {
                var mods = typeof(FluXisGameBase).Assembly.GetTypes()
                                                 .Where(t => t.GetInterfaces().Contains(typeof(IMod))).ToList()
                                                 .Select(t => (IMod)Activator.CreateInstance(t));

                foreach (var mod in mods)
                {
                    var name = modstr.GetName(mod);
                    var description = modstr.GetDescription(mod);

                    var (nameKey, nameDefault) = getValues(name);
                    var (descriptionKey, descriptionDefault) = getValues(description);

                    dict.Add(nameKey, nameDefault);
                    words += nameDefault.Split(" ").Length;
                    count++;

                    dict.Add(descriptionKey, descriptionDefault);
                    words += descriptionDefault.Split(" ").Length;
                    count++;
                }
            }

            foreach (var prop in props)
            {
                var val = prop.GetValue(cat)!;
                TranslatableString value = null!;

                if (prop.PropertyType == typeof(LocalisableString))
                {
                    var localType = typeof(LocalisableString);
                    var dataField = localType.GetField("Data", BindingFlags.Instance | BindingFlags.NonPublic);
                    var data = (TranslatableString)dataField!.GetValue(val);
                    value = data;
                }
                else if (prop.PropertyType == typeof(TranslatableString))
                    value = (TranslatableString)val;

                if (value is null)
                    continue;

                var (key, defaultStr) = getValues(value);
                dict.Add(key, defaultStr);

                words += defaultStr.Split(" ").Length;
                count++;
            }

            var json = dict.Serialize(true);
            File.WriteAllText($"../../../../fluXis.Resources/Localization/en/{file}.json", json);
        }

        Logger.Log($"Wrote {count} strings with {words} words.");

        (string, string) getValues(TranslatableString str) => (str.Key.Split(':')[1], str.Format);
    }
}
