using System.Collections.Generic;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.HitObjects;

public class ValidKeyCountCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        if (ctx.RealmMap.KeyCount is < 4 or > 8)
        {
            yield return new VerifyIssue(
                VerifyIssueSeverity.Warning,
                VerifyIssueCategory.HitObjects,
                null,
                "Keycount is invalid. It must be between 4 and 8. You can upload the map but not submit it for pure."
            );
        }
    }
}
