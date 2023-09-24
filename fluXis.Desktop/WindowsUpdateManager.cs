using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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

    private readonly Logger logger = Logger.GetLogger("update");

    private const string updater_path = @"updater\fluXis.Updater.exe";

    [BackgroundDependencyLoader]
    private async void load()
    {
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

        if (!File.Exists(updater_path))
        {
            var notification = new LoadingNotification
            {
                TextLoading = "Downloading updater...",
                TextFailure = "Failed to download updater. Check update.log for more information.",
                TextSuccess = "Updater downloaded. Launching in 5 seconds.",
            };
            notifications.AddNotification(notification);

            const string url = "https://dl.choccy.foxes4life.net/fluXis/updater.zip";
            var request = new WebRequest(url);
            request.DownloadProgress += (currentBytes, totalBytes) => notification.Progress = (float)currentBytes / totalBytes;
            request.Failed += e =>
            {
                logger.Add($"Failed to download updater. {e.Message}");
                notification.State = LoadingState.Failed;
            };
            request.Finished += () =>
            {
                Directory.CreateDirectory("updater");

                var bytes = request.GetResponseData();
                using var stream = new MemoryStream(bytes);

                using var zip = new ZipArchive(stream);
                zip.ExtractToDirectory("updater", true);

                notification.State = LoadingState.Loaded;
                Task.Delay(5000).ContinueWith(_ => startUpdater());
            };

            try { await request.PerformAsync(); }
            catch (Exception e)
            {
                logger.Add("Failed to download updater.", LogLevel.Error, e);
                notification.State = LoadingState.Failed;
            }
        }
        else
        {
            notifications.Post("There is an update available.\nLaunching updater in 5 seconds.");
            await Task.Delay(5000);
            startUpdater();
        }
    }

    private void startUpdater()
    {
        try
        {
            // start in update folder
            Process.Start(new ProcessStartInfo
            {
                FileName = @$"{Environment.CurrentDirectory}\{updater_path}",
                WorkingDirectory = "updater"
            });
            Environment.Exit(0);
        }
        catch (Exception e)
        {
            logger.Add($"Failed to launch updater. {e.Message}");
            notifications.PostError("Failed to launch updater.\nPlease download the latest version from GitHub or download the updater from the website.");
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
