using System.Collections.Generic;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Verify;

namespace fluXis.Modes.Keys.Editor.Checks;

public class UnevenInEvenCountCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        var count = ctx.RealmMap.KeyCount;

        // skip if the map is not even
        if (count % 2 != 0)
            yield break;

        foreach (var switchEvent in ctx.Map.ObjectsOfType<LaneSwitchEvent>())
        {
            // skip if the is even
            if (switchEvent.Count % 2 == 0)
                continue;

            yield return new VerifyIssue(
                VerifyIssueSeverity.Warning,
                VerifyIssueCategory.Effects,
                switchEvent.Time,
                "A lane switch is using an uneven count in an even keycount map. Consider changing the map's keycount to odd or using an even count for the lane switch."
            );
        }
    }
}
