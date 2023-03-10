using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Map.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.Overlay.Effect;

public partial class FlashOverlay : Container
{
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

    public void Flash(float fadeInTime, float holdTime, float fadeOutTime)
    {
        flash.FadeIn(fadeInTime).Then().Delay(holdTime).FadeOut(fadeOutTime);
    }

    protected override void Update()
    {
        while (flashes.Count > 0 && flashes[0].Time <= Conductor.CurrentTime)
        {
            Flash(flashes[0].FadeInTime, flashes[0].HoldTime, flashes[0].FadeOutTime);
            flashes.RemoveAt(0);
        }
    }
}
