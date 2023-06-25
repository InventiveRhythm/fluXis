using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using fluXis.Game.IPC;
using osu.Framework.Platform;
using osu.Framework;
using Squirrel;

namespace fluXis.Desktop;

public static class Program
{
    public static string[] Args { get; private set; } = Array.Empty<string>();

    public static void Main(string[] args)
    {
        Args = args;

        if (OperatingSystem.IsWindows())
        {
            setupSquirrel();
            FileExtensionHelper.EnsureAssociationsSet();
        }

        string name = @"fluXis";

        if (args.Contains("--dev")) name += "-dev";

        using GameHost host = Host.GetSuitableDesktopHost(name, new HostOptions { BindIPC = true });

        if (host.IsPrimaryInstance)
        {
            var ipc = new TcpIpcProvider(24242);
            ipc.Bind();

            var game = new FluXisGameDesktop();
            host.Run(game);
        }
        else sendIpcMessage(host, args);
    }

    private static void sendIpcMessage(IIpcHost host, IReadOnlyList<string> args)
    {
        if (args.Count <= 0 || !args[0].Contains('.')) return;

        foreach (string file in args)
        {
            var channel = new IPCImportChannel(host);
            channel.Import(file).Wait(3000);
        }
    }

    [SupportedOSPlatform("windows")]
    private static void setupSquirrel()
    {
        SquirrelAwareApp.HandleEvents((_, tools) =>
        {
            tools.CreateShortcutForThisExe();
            tools.CreateUninstallerRegistryEntry();
        }, (_, tools) =>
        {
            tools.CreateUninstallerRegistryEntry();
        }, (_, _) =>
        {
        }, (_, tools) =>
        {
            tools.RemoveShortcutForThisExe();
            tools.RemoveUninstallerRegistryEntry();
        }, (_, _, _) =>
        {
        });
    }
}
