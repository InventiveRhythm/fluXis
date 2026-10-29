using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Modes.Keys.Objects;
using fluXis.Screens.Edit.Tabs.Verify;
using osu.Framework.Utils;

namespace fluXis.Modes.Keys.Editor.Checks;

public class OverlappingCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        var hits = ctx.Map.ObjectsOfType<HitObject>();
        if (hits.Length == 0) yield break;

        foreach (var lane in hits.GroupBy(h => h.Lane))
        {
            var hitObjects = lane.ToList();
            if (hitObjects.Count == 0) continue;

            HitObject? currentLn = null;

            foreach (var (prev, current) in hitObjects.Zip(hitObjects.Skip(1)))
            {
                if (((prev as IHasDuration)?.Duration ?? 0) > 0 && (currentLn == null || prev.GetEndTime() > currentLn.GetEndTime()))
                    currentLn = prev;

                bool overlappingTime = Precision.AlmostEquals(current.Time, prev.Time);
                bool overlappingEndTime = currentLn != null && Precision.AlmostEquals(current.Time, currentLn.GetEndTime());
                bool insideLn = currentLn != null && current is Tick // except for tick notes inside LNs
                                                  && current.Time > currentLn.Time && current.Time < currentLn.GetEndTime();

                if (overlappingTime || insideLn || overlappingEndTime)
                {
                    yield return new VerifyIssue(
                        VerifyIssueSeverity.Warning,
                        VerifyIssueCategory.HitObjects,
                        current.Time,
                        $"Overlapping notes at lane {lane.Key}."
                    );
                }

                if (currentLn != null && current.Time >= currentLn.GetEndTime())
                    currentLn = null;
            }
        }
    }
}
