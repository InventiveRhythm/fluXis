using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using fluXis.Game;
using fluXis.Game.Overlay.Notification;
using Newtonsoft.Json.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Desktop;

[SupportedOSPlatform("windows")]
public partial class WindowsUpdateManager : Component
{
    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [BackgroundDependencyLoader]
    private async void load()
    {
        var logger = Logger.GetLogger("update");
        var current = FluXisGameBase.Version;

        if (current == null)
        {
            logger.Add("No version installed.");
            return;
        }

        if (FluXisGameBase.IsDebug)
        {
            logger.Add("Running in debug mode. Skipping update check.");
            return;
        }

        logger.Add($"Current version: {current}");
        logger.Add("Checking for updates...");

        var latest = fetchLatestVersion();

        if (!Version.TryParse(latest, out var latestVersion))
        {
            logger.Add($"Failed to parse latest version. {latestVersion}");
            return;
        }

        logger.Add($"Latest version: {latestVersion}");

        if (current >= latestVersion)
        {
            logger.Add("No updates available.");
            return;
        }

        const string updater_path = "Updater.exe";

        if (!System.IO.File.Exists(updater_path))
        {
            logger.Add("Updater.exe not found.");
            notifications.PostError("There is an update available.\nBut the updater is missing.\nPlease download the latest version from GitHub or download the updater from the website.");
        }
        else
        {
            notifications.Post("There is an update available.\nLaunching updater in 5 seconds.");

            await Task.Delay(5000);

            try
            {
                Process.Start("Updater.exe");
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                logger.Add($"Failed to launch updater. {e.Message}");
                notifications.PostError("Failed to launch updater.\nPlease download the latest version from GitHub or download the updater from the website.");
            }
        }
    }

    private string fetchLatestVersion()
    {
        const string url = "https://api.github.com/repos/TeamfluXis/fluXis/releases/latest";
        var request = new WebRequest(url);
        request.AddHeader("User-Agent", "fluXis.Updater");
        request.Method = HttpMethod.Get;
        request.Perform();

        var json = JObject.Parse(request.GetResponseString());
        return json["tag_name"]?.ToString() ?? "";
    }
}
