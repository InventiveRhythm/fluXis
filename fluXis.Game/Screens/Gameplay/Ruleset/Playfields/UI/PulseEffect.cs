using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Gameplay.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.Ruleset.Playfields.UI;

public partial class PulseEffect : Container
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private GameplayClock clock { get; set; }

    private List<PulseEvent> pulses = new();

    public PulseEffect()
    {
        RelativeSizeAxes = Axes.Both;
    }

    protected override void LoadComplete()
    {
        pulses = screen.MapEvents.PulseEvents.ToList();
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

            left.MoveToX(-400, clock.BeatTime, Easing.OutQuint).FadeOut(clock.BeatTime).Expire();
            right.MoveToX(400, clock.BeatTime, Easing.OutQuint).FadeOut(clock.BeatTime).Expire();
        }
    }
}
