using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using fluXis.Configuration;
using fluXis.Updater.GitHub;
using fluXis.Utils;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Updater;

public class UpdateChecker
{
    private Logger logger { get; } = Logger.GetLogger("update");
    private ReleaseChannel channel { get; }

    private string latestStr = "";

    public string LatestVersion => string.IsNullOrEmpty(latestStr) ? latestStr = fetchLatestVersion() : latestStr;

    public bool UpdateAvailable
    {
        get
        {
            var current = FluXisGameBase.Version;

            logger.Add("Checking for updates...");
            logger.Add($"Current version: {current}");

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

            if (!Version.TryParse(LatestVersion, out var latest))
            {
                logger.Add($"Failed to parse latest version. {LatestVersion}");
                return false;
            }

            logger.Add($"Latest version: {latest}");

            if (current < latest)
                return true;

            logger.Add("No updates available.");
            return false;
        }
    }

    public UpdateChecker(ReleaseChannel channel)
    {
        this.channel = channel;
    }

    private string fetchLatestVersion()
    {
        const string url = "https://api.github.com/repos/InventiveRhythm/fluXis/releases";
        var request = new WebRequest(url);
        request.AddHeader("User-Agent", "fluXis.Updater");
        request.Method = HttpMethod.Get;
        request.Perform();

        var releases = request.GetResponseString().Deserialize<List<GitHubRelease>>();

        if (releases.Count == 0)
        {
            logger.Add("No releases found.");
            return "0.0.0";
        }

        return channel == ReleaseChannel.Stable
            ? releases.First(r => !r.PreRelease).TagName
            : releases[0].TagName;
    }
}
