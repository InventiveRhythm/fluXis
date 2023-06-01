using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Map.Events;
using OpenTabletDriver.Plugin.DependencyInjection;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.Overlay.Effect;

public partial class FlashOverlay : Container
{
    [Resolved]
    private AudioClock clock { get; set; }

    private readonly List<FlashEvent> flashes;
    private readonly Box flash;

    public FlashOverlay(List<FlashEvent> flashes)
    {
        this.flashes = flashes;
        RelativeSizeAxes = Axes.Both;

        AddInternal(flash = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.White,
            Alpha = 0
        });
    }

    protected override void Update()
    {
        while (flashes.Count > 0 && flashes[0].Time <= clock.CurrentTime)
        {
            var flashEvent = flashes[0];

            flash.FadeColour(flashEvent.StartColor);
            flash.FadeTo(flashEvent.StartOpacity);

            flash.FadeColour(flashEvent.EndColor, flashEvent.Duration, flashEvent.Easing);
            flash.FadeTo(flashEvent.EndOpacity, flashEvent.Duration, flashEvent.Easing);

            flashes.RemoveAt(0);
        }
    }
}
