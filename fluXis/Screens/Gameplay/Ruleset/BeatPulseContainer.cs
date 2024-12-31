using System;
using System.Collections.Generic;
using fluXis.Map;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Gameplay.Ruleset;

public partial class BeatPulseContainer : CompositeDrawable
{
    private MapInfo map { get; }
    private List<BeatPulseEvent> events { get; }
    private Drawable target { get; }

    private float targetScale
    {
        get => target.Scale.X;
        set => target.Scale = new Vector2(value);
    }

    public BeatPulseContainer(MapInfo map, List<BeatPulseEvent> events, Drawable target)
    {
        this.map = map;
        this.events = events;
        this.target = target;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        generate();
    }

    private void generate()
    {
        for (int i = 0; i < events.Count; i++)
        {
            var ev = events[i];

            if (Math.Abs(ev.Strength - 1) < 0.0001f || ev.Interval < 0.01f)
                continue;

            var end = i + 1 < events.Count ? events[i + 1].Time : map.EndTime;

            var t = ev.Time;

            while (t < end)
            {
                var timing = map.GetTimingPoint(t);
                var ms = timing.MsPerBeat * Math.Clamp(ev.Interval, 0.01f, 4);

                var inDuration = ms * ev.ZoomIn;
                var outDuration = ms * (1 - ev.ZoomIn);

                using (BeginAbsoluteSequence(t))
                {
                    this.TransformTo(nameof(targetScale), ev.Strength, inDuration, Easing.OutQuint)
                        .Then().TransformTo(nameof(targetScale), 1f, outDuration, Easing.OutQuint);
                }

                t += ms;
            }
        }
    }
}
