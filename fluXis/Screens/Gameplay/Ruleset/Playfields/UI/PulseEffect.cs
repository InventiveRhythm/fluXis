using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields.UI;

public partial class PulseEffect : Container
{
    [Resolved]
    private RulesetContainer ruleset { get; set; }

    [Resolved]
    private IBeatSyncProvider beatSync { get; set; }

    private List<PulseEvent> pulses = new();

    public PulseEffect()
    {
        RelativeSizeAxes = Axes.Both;
    }

    protected override void LoadComplete()
    {
        pulses = ruleset.MapEvents.PulseEvents.ToList();
        pulses.Sort((x, y) => x.Time.CompareTo(y.Time));
    }

    protected override void Update()
    {
        if (pulses.Count == 0) return;

        var time = Clock.CurrentTime;

        if (time >= pulses[0].Time)
        {
            pulses.RemoveAt(0);

            var left = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 4,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight
            };

            var right = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 4,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreLeft
            };

            Add(left);
            Add(right);

            left.MoveToX(-400, beatSync.BeatTime, Easing.OutQuint).FadeOut(beatSync.BeatTime).Expire();
            right.MoveToX(400, beatSync.BeatTime, Easing.OutQuint).FadeOut(beatSync.BeatTime).Expire();
        }
    }
}
