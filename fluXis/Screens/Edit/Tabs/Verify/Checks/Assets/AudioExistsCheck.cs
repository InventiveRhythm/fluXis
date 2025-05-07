using System.Collections.Generic;
using System.IO;
using fluXis.Database;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.Assets;

public class AudioExistsCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        var path = ctx.MapSet.GetPathForFile(ctx.MapInfo.AudioFile);
        path = MapFiles.GetFullPath(path);

        if (string.IsNullOrEmpty(path))
        {
            yield return new VerifyIssue(
                VerifyIssueSeverity.Problematic,
                VerifyIssueCategory.Audio,
                null,
                "No audio file has been set."
            );

            yield break;
        }

        if (!File.Exists(path))
        {
            yield return new VerifyIssue(
                VerifyIssueSeverity.Problematic,
                VerifyIssueCategory.Audio,
                null,
                "The audio file does not exist."
            );
        }
    }
}
