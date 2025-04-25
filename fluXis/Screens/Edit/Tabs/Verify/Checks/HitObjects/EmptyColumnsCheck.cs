using System.Collections.Generic;
using System.Linq;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.HitObjects;

public class EmptyColumnsCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(EditorMap map)
    {
        var hits = map.MapInfo.HitObjects;
        if (hits.Count == 0) yield break;

        var count = map.MaxKeyCount;

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
