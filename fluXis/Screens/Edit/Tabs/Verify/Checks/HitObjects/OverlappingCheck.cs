using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using osu.Framework.Utils;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks.HitObjects;

public class OverlappingCheck : IVerifyCheck
{
    public IEnumerable<VerifyIssue> Check(IVerifyContext ctx)
    {
        var hits = ctx.MapInfo.HitObjects;
        if (hits.Count == 0) yield break;

        foreach (var lane in hits.GroupBy(h => h.Lane))
        {
            var hitObjects = lane.ToList();
            if (hitObjects.Count == 0) continue;

            HitObject currentLn = null;

            foreach (var (prev, current) in hitObjects.Zip(hitObjects.Skip(1)))
            {
                if (prev.HoldTime > 0 && (currentLn == null || prev.EndTime > currentLn.EndTime))
                    currentLn = prev;

                bool overlappingTime = Precision.AlmostEquals(current.Time, prev.Time);
                bool overlappingEndTime = currentLn != null && Precision.AlmostEquals(current.Time, currentLn.EndTime);
                bool insideLn = currentLn != null && current.Type != 1 // except for tick notes inside LNs
                                                  && current.Time > currentLn.Time && current.Time < currentLn.EndTime;

                if (overlappingTime || insideLn || overlappingEndTime)
                {
                    yield return new VerifyIssue(
                        VerifyIssueSeverity.Warning,
                        VerifyIssueCategory.HitObjects,
                        current.Time,
                        $"Overlapping notes at lane {lane.Key}."
                    );
                }

                if (currentLn != null && current.Time >= currentLn.EndTime)
                    currentLn = null;
            }
        }
    }
}
