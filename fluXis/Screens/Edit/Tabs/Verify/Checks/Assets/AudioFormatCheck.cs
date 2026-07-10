using System.Collections.Generic;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.Assets;

public class AudioFormatCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        using var track = ctx.RealmMap.GetTrack();

        if (track is null)
            yield return new VerifyIssue(VerifyIssueSeverity.Problematic, VerifyIssueCategory.Audio, null, "Could not load audio file.");
        else
        {
            track.StopAsync().Wait(); // this is needed, don't ask me why

            if (track.Bitrate is null or <= 0)
                yield return new VerifyIssue(VerifyIssueSeverity.Problematic, VerifyIssueCategory.Audio, null, "Could not get audio bitrate.");
            else if (track.Bitrate is < 128 or > 192)
                yield return new VerifyIssue(VerifyIssueSeverity.Warning, VerifyIssueCategory.Audio, null, $"Audio bitrate must be between 128kbps and 192kbps. (Current: {track.Bitrate}kbps)");
        }
    }
}
