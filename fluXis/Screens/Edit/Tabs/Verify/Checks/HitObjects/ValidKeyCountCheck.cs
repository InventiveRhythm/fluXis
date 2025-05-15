using System.Collections.Generic;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.HitObjects;

public class ValidKeyCountCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        if (ctx.RealmMap.KeyCount < 4 || ctx.RealmMap.KeyCount > 8)
        {
            yield return new VerifyIssue(
                VerifyIssueSeverity.Problematic,
                VerifyIssueCategory.HitObjects,
                null,
                "Keycount is invalid. It must be between 4 and 8."
            );
        }
    }
}
