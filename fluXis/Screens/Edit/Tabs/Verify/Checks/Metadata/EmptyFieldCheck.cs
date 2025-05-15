using System.Collections.Generic;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.Metadata;

public class EmptyFieldCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        var info = ctx.MapInfo;
        var metadata = info.Metadata;

        if (string.IsNullOrWhiteSpace(metadata.Title))
        {
            yield return new VerifyIssue(
                VerifyIssueSeverity.Problematic,
                VerifyIssueCategory.Metadata,
                null,
                "Title is empty."
            );
        }

        if (string.IsNullOrWhiteSpace(metadata.Artist))
        {
            yield return new VerifyIssue(
                VerifyIssueSeverity.Problematic,
                VerifyIssueCategory.Metadata,
                null,
                "Artist is empty."
            );
        }

        if (string.IsNullOrWhiteSpace(metadata.Difficulty))
        {
            yield return new VerifyIssue(
                VerifyIssueSeverity.Problematic,
                VerifyIssueCategory.Metadata,
                null,
                "Difficulty name is empty."
            );
        }
    }
}
