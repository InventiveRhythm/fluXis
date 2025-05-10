using System;
using System.Collections.Generic;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Edit.Tabs.Design.Effects;

public partial class EditorFlashLayer : CompositeDrawable
{
    public override bool RemoveCompletedTransforms => false;

    private Box box;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChild = box = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Alpha = 0
        };
    }

    public void Rebuild(List<FlashEvent> flashes)
    {
        ClearTransforms();
        box.FadeOut();

        flashes.ForEach(flash =>
        {
            using (BeginAbsoluteSequence(flash.Time))
            {
                var dur = Math.Max(flash.Duration, 0);

                box.FadeColour(flash.StartColor).FadeTo(flash.StartOpacity)
                   .FadeColour(flash.EndColor, dur, flash.Easing)
                   .FadeTo(flash.EndOpacity, dur, flash.Easing);
            }
        });
    }
}
