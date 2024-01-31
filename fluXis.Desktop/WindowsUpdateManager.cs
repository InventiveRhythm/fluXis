using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Versioning;
using fluXis.Game;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Updater;
using Newtonsoft.Json.Linq;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Desktop;

[SupportedOSPlatform("windows")]
public partial class WindowsUpdateManager : IUpdateManager
{
    private NotificationManager notifications { get; }
    private readonly Logger logger = Logger.GetLogger("update");

    private string latestVersion;
    private bool forceUpdate;

    private string folder => $"{Path.GetDirectoryName(Environment.CommandLine)}";
    private string patcher => @$"{folder}\patcher.exe";
    private string patches => @$"{folder}\patches";

    public bool UpdateAvailable
    {
        get
        {
            var current = FluXisGameBase.Version;

            logger.Add("Checking for updates...");
            logger.Add($"Current version: {current}");

            if (!forceUpdate)
            {
                if (current == null)
                {
                    logger.Add("No version installed.");
                    return false;
                }

                if (FluXisGameBase.IsDebug)
                {
                    logger.Add("Running in debug mode. Skipping update check.");
                    return false;
                }
            }

            var latestString = fetchLatestVersion();

            if (!Version.TryParse(latestString, out var latest))
            {
                logger.Add($"Failed to parse latest version. {latestString}");
                return false;
            }

            latestVersion = latestString;
            logger.Add($"Latest version: {latestVersion}");

            if (current < latest) return true;

            logger.Add("No updates available.");
            return false;
        }
    }

    public WindowsUpdateManager(NotificationManager notifications)
    {
        this.notifications = notifications;
    }

    public void Perform(bool silent, bool forceUpdate = false)
    {
        if (Program.Args.Contains("--post-update"))
        {
            logger.Add("Update complete. Cleaning up...");

            if (Directory.Exists(patches))
                Directory.Delete(patches, true);

            logger.Add("Cleanup complete.");
        }

        this.forceUpdate = forceUpdate;

        if (!UpdateAvailable && !forceUpdate)
        {
            if (!silent)
                notifications.SendText("No updates available.", "You are running the latest version.", FontAwesome6.Solid.Check);

            return;
        }

        if (string.IsNullOrEmpty(latestVersion))
            return;

        if (!File.Exists(patcher))
            getPatcher(() => startUpdate(latestVersion));
        else
            startUpdate(latestVersion);
    }

    public void UpdateFromFile(FileInfo file)
    {
        if (!File.Exists(patcher))
        {
            getPatcher(() => UpdateFromFile(file));
            return;
        }

        try
        {
            // open the as zip and check if it contains a fluXis.exe
            using var zip = ZipFile.OpenRead(file.FullName);

            if (zip.Entries.All(e => e.Name != "fluXis.exe"))
            {
                logger.Add("Invalid update file.");
                notifications.SendError("Invalid update file.");
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = patcher,
                UseShellExecute = true,
                Arguments = $"{file.FullName} {folder}",
                WorkingDirectory = folder
            });
            Environment.Exit(0);
        }
        catch (Exception e)
        {
            logger.Add("Failed to install update.", LogLevel.Error, e);
            notifications.SendError("Failed to install update.", "Check update.log for more information.");
        }
    }

    private void startUpdate(string latest)
    {
        var notification = new TaskNotificationData
        {
            Text = "New update available!",
            TextWorking = "Downloading...",
            TextFailed = "Failed! Check update.log for more information.",
            TextFinished = "Done! Starting update..."
        };

        notifications.AddTask(notification);

        try
        {
            var request = new WebRequest($"https://dl.flux.moe/fluXis/{latest}.zip");
            request.DownloadProgress += (currentBytes, totalBytes) => notification.Progress = (float)currentBytes / totalBytes;
            request.Failed += e =>
            {
                logger.Add($"Failed to download update. {e.Message}", LogLevel.Error, e);
                notifications.SendError("Failed to download update.", "Check update.log for more information.");
                notification.State = LoadingState.Failed;
            };
            request.Finished += () =>
            {
                logger.Add("Downloaded update. Starting update...");
                Directory.CreateDirectory(patches);

                var bytes = request.GetResponseData();
                using var stream = new MemoryStream(bytes);

                var path = patches + $"/{latest}.zip";
                File.WriteAllBytes(path, bytes);

                notification.State = LoadingState.Complete;
                logger.Add("Update complete. Launching patcher...");
                UpdateFromFile(new FileInfo(path));
            };

            request.Perform();
        }
        catch (Exception e)
        {
            logger.Add($"Failed to download update. {e.Message}");
            notifications.SendError("Failed to download update.", "Check update.log for more information.");
            notification.State = LoadingState.Failed;
        }
    }

    private async void getPatcher(Action callback)
    {
        var notification = new TaskNotificationData()
        {
            Text = "Game patcher download",
            TextWorking = "Downloading...",
            TextFailed = "Failed! Check update.log for more information.",
            TextFinished = "Done! Starting update...",
        };

        notifications.AddTask(notification);

        const string url = "https://dl.flux.moe/fluXis/patcher.exe";
        var request = new WebRequest(url);
        request.DownloadProgress += (currentBytes, totalBytes) => notification.Progress = (float)currentBytes / totalBytes;
        request.Failed += e =>
        {
            logger.Add($"Failed to download patcher. {e.Message}");
            notification.State = LoadingState.Failed;
        };
        request.Finished += () =>
        {
            Directory.CreateDirectory(folder);

            var bytes = request.GetResponseData();
            using var stream = new MemoryStream(bytes);
            File.WriteAllBytes(patcher, bytes);

            notification.State = LoadingState.Complete;
            callback?.Invoke();
        };

        try { await request.PerformAsync(); }
        catch (Exception e)
        {
            logger.Add("Failed to download patcher.", LogLevel.Error, e);
            notification.State = LoadingState.Failed;
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
