using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class PulseEffect : Container
{
    public GameplayScreen ParentScreen { get; set; }

    private List<PulseEvent> pulses = new();

    public PulseEffect()
    {
        RelativeSizeAxes = Axes.Both;
    }

    protected override void LoadComplete()
    {
        pulses = ParentScreen.MapEvents.PulseEvents.ToList();
        pulses.Sort((x, y) => x.Time.CompareTo(y.Time));
    }

    protected override void Update()
    {
        if (pulses.Count == 0) return;

        var time = ParentScreen.AudioClock.CurrentTime;

        if (time >= pulses[0].Time)
        {
            pulses.RemoveAt(0);

            var width = ParentScreen.Playfield.Stage.Width;

            var left = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 4,
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreRight,
                X = -width / 2,
            };

            var right = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 4,
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                X = width / 2,
            };

            Add(left);
            Add(right);

            left.MoveToX(-width / 2 - 400, ParentScreen.AudioClock.BeatTime, Easing.OutQuint).FadeOut(ParentScreen.AudioClock.BeatTime).Expire();
            right.MoveToX(width / 2 + 400, ParentScreen.AudioClock.BeatTime, Easing.OutQuint).FadeOut(ParentScreen.AudioClock.BeatTime).Expire();
        }
    }
}
