using System;
using System.Runtime.Versioning;
using osu.Framework.Platform;
using osu.Framework;
using Squirrel;

namespace fluXis.Desktop;

public static class Program
{
    public static void Main()
    {
        if (OperatingSystem.IsWindows())
            setupSquirrel();

        using GameHost host = Host.GetSuitableDesktopHost(@"fluXis");
        using osu.Framework.Game game = new FluXisGameDesktop();
        host.Run(game);
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
