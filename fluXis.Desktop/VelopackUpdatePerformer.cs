using fluXis.Game;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Updater;
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
        var mgr = new UpdateManager(new GithubSource("https://github.com/TeamFluXis/fluXis", "", beta));

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
}
