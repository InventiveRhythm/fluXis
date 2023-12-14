using System.Collections.Generic;
using fluXis.Game.Configuration;
using fluXis.Game.Map.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Overlay.Effect;

public partial class FlashOverlay : Container
{
    private readonly List<FlashEvent> flashes;
    private readonly Box flash;

    public FlashOverlay(List<FlashEvent> flashes)
    {
        this.flashes = flashes;
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;
        Size = new Vector2(2f);

        AddInternal(flash = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.White,
            Alpha = 0
        });
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        Alpha = config.Get<bool>(FluXisSetting.DisableEpilepsyIntrusingEffects) ? 0 : 1;
    }

    protected override void Update()
    {
        while (flashes.Count > 0 && flashes[0].Time <= Clock.CurrentTime)
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
