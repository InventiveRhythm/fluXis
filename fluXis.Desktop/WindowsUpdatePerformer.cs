using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Versioning;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Updater;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Desktop;

[SupportedOSPlatform("windows")]
public partial class WindowsUpdatePerformer : IUpdatePerformer
{
    private NotificationManager notifications { get; }
    private readonly Logger logger = Logger.GetLogger("update");

    private string folder => $"{AppContext.BaseDirectory}";
    private string patcher => @$"{folder}\patcher.exe";
    private string patches => @$"{folder}\patches";

    public WindowsUpdatePerformer(NotificationManager notifications)
    {
        this.notifications = notifications;
    }

    public void Perform(string version)
    {
        if (string.IsNullOrEmpty(version))
            return;

        if (!File.Exists(patcher))
            getPatcher(() => startUpdate(version));
        else
            startUpdate(version);
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

            // the backslash to forward slash is absolutely needed here
            // else the patcher thinks the closing quote of the folder
            // is escaped since it ends in \
            Process.Start(new ProcessStartInfo
            {
                FileName = patcher,
                UseShellExecute = true,
                Arguments = $"\"{file.FullName.Replace("\\", "/")}\" \"{folder.Replace("\\", "/")}\" ",
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
        var notification = new TaskNotificationData
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
}
