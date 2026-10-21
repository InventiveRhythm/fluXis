using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Tabs.Verify;

namespace fluXis.Mode.Keys.Editor.Checks;

public class EmptyColumnsCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        var hits = ctx.Map.ObjectsOfType<HitObject>();
        if (hits.Length == 0) yield break;

        var count = ctx.Configuration.ColumnCount;

        for (int i = 0; i < count; i++)
        {
            var hasNotes = hits.Any(hit => hit.Lane == i + 1);

            if (!hasNotes)
            {
                yield return new VerifyIssue(
                    VerifyIssueSeverity.Warning,
                    VerifyIssueCategory.HitObjects,
                    null,
                    $"Lane {i + 1} is empty."
                );
            }
        }
    }
}
