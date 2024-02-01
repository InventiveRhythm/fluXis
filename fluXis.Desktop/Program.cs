using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game;
using fluXis.Game.IPC;
using osu.Framework.Platform;
using osu.Framework;
using osu.Framework.Logging;

namespace fluXis.Desktop;

public static class Program
{
    public static string[] Args { get; private set; } = Array.Empty<string>();

    public static void Main(string[] args)
    {
        Args = args;

        if (OperatingSystem.IsWindows())
            FileExtensionHelper.EnsureAssociationsSet();

        string name = $"fluXis{(args.Contains("--dev") ? "-dev" : "")}";

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
}
