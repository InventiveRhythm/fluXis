using System.Collections.Generic;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Gameplay.Overlay.Effect;

public partial class FlashOverlay : Container
{
    private List<FlashEvent> flashes { get; }
    private Box box { get; }

    public FlashOverlay(List<FlashEvent> flashes)
    {
        this.flashes = flashes;
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;
        Size = new Vector2(2f);

        AddInternal(box = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.White,
            Alpha = 0
        });
    }

    protected override void Update()
    {
        while (flashes.Count > 0 && flashes[0].Time <= Clock.CurrentTime)
        {
            var flashEvent = flashes[0];

            box.FadeColour(flashEvent.StartColor);
            box.FadeTo(flashEvent.StartOpacity);

            box.FadeColour(flashEvent.EndColor, flashEvent.Duration, flashEvent.Easing);
            box.FadeTo(flashEvent.EndOpacity, flashEvent.Duration, flashEvent.Easing);

            flashes.RemoveAt(0);
        }
    }
}
