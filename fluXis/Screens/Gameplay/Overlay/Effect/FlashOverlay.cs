using System;
using System.Collections.Generic;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Gameplay.Overlay.Effect;

public partial class FlashOverlay : CompositeDrawable
{
    private List<FlashEvent> flashes { get; }

    public FlashOverlay(List<FlashEvent> flashes)
    {
        this.flashes = flashes;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;
        Size = new Vector2(2f);

        Box box;

        InternalChild = box = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.White,
            Alpha = 0
        };

        foreach (var flash in flashes)
        {
            using (BeginAbsoluteSequence(flash.Time))
            {
                var dur = Math.Max(flash.Duration, 0);

                box.FadeColour(flash.StartColor).FadeTo(flash.StartOpacity)
                   .FadeColour(flash.EndColor, dur, flash.Easing)
                   .FadeTo(flash.EndOpacity, dur, flash.Easing);
            }
        }
    }
}
