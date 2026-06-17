using System.Collections.Generic;
using System.Linq;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.Assets;

public class PreviewPointCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        if (ctx.MapInfo.Metadata.PreviewTime == 0)
            yield return new VerifyIssue(VerifyIssueSeverity.Warning, VerifyIssueCategory.Audio, null, "No preview point is set.");

        var reference = ctx.MapInfo.Metadata.PreviewTime;
        if (ctx.MapSet.Maps.Any(x => x.Metadata.PreviewTime != reference))
            yield return new VerifyIssue(VerifyIssueSeverity.Warning, VerifyIssueCategory.Audio, null, "Preview point is inconsistent across difficulties.");
    }
}
