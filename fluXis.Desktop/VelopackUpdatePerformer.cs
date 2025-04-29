#if VELOPACK_BUILD
using fluXis.Graphics.Sprites;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Updater;
using osu.Framework.Logging;
using Velopack;
using Velopack.Sources;

namespace fluXis.Desktop;

public partial class VelopackUpdatePerformer : IUpdatePerformer
{
    private NotificationManager notifications { get; }
    private readonly Logger logger = Logger.GetLogger("update");

    public VelopackUpdatePerformer(NotificationManager notifications)
    {
        this.notifications = notifications;
    }

    public void Perform(bool silent, bool beta)
    {
        if (FluXisGameBase.IsDebug)
        {
            logger.Add("Skipping update in debug.");
            return;
        }

        logger.Add("Checking for updates...");
        var mgr = new UpdateManager(new GithubSource("https://github.com/InventiveRhythm/fluXis", "", beta));

        var update = mgr.CheckForUpdates();

        if (update is null)
        {
            logger.Add("No update found.");

            if (!silent)
                notifications.SendText("No updates available.", "You are running the latest version.", FontAwesome6.Solid.Check);

            return;
        }

        var notification = new TaskNotificationData
        {
            Text = "New update available!",
            TextWorking = "Downloading...",
            TextFailed = "Failed! Check update.log for more information.",
            TextFinished = "Done! Starting update..."
        };

        notifications.AddTask(notification);
        logger.Add($"Downloading {update.TargetFullRelease.Version}...");
        mgr.DownloadUpdates(update, i => notification.Progress = i / 100f);

        logger.Add("Applying...");
        notification.State = LoadingState.Complete;
        mgr.ApplyUpdatesAndRestart(update);
    }

    public bool RestartOnClose()
    {
        try
        {
            UpdateExe.Start();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
#endif
