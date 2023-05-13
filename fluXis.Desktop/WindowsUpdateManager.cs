using System;
using System.Runtime.Versioning;
using fluXis.Game;
using fluXis.Game.Overlay.Notification;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using Squirrel;

namespace fluXis.Desktop;

[SupportedOSPlatform("windows")]
public partial class WindowsUpdateManager : Component
{
    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [BackgroundDependencyLoader]
    private async void load()
    {
        var manager = new GithubUpdateManager(@"https://github.com/TeamFluXis/fluXis", false, null, "fluXis");

        if (manager.CurrentlyInstalledVersion() is { } version)
            Logger.Log($"Current version: {version}", LoggingTarget.Runtime, LogLevel.Important);
        else
        {
            Logger.Log("No version installed.", LoggingTarget.Runtime, LogLevel.Important);
            return;
        }

        Logger.Log("Checking for updates...", LoggingTarget.Runtime, LogLevel.Important);
        var info = await manager.CheckForUpdate().ConfigureAwait(false);

        if (info.ReleasesToApply.Count == 0)
        {
            Logger.Log("No updates available.", LoggingTarget.Runtime, LogLevel.Important);
            return;
        }

        try
        {
            notifications.Post("An update is available. Downloading...");
            await manager.DownloadReleases(info.ReleasesToApply).ConfigureAwait(false);
            notifications.Post("Applying update...");
            await manager.ApplyReleases(info).ConfigureAwait(false);
            notifications.Post("Updates applied. Restarting...");
            FluXisGame.ExitGame();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Update failed.");
            notifications.Post("Update failed. Check logs for more information.");
            throw;
        }
    }
}
