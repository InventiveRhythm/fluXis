using fluXis.Game.Map.Events;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Effect;

public partial class EditorFlashEvent : CircularContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    public FlashEvent FlashEvent { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 10;
        Height = .5f * (FlashEvent.Duration * values.Zoom);
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;
        Masking = true;

        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = ColourInfo.GradientVertical(FlashEvent.EndColor.Opacity(FlashEvent.EndOpacity), FlashEvent.StartColor.Opacity(FlashEvent.StartOpacity))
        };
    }

    protected override void Update()
    {
        Y = -((FlashEvent.Time - (float)clock.CurrentTime) * values.Zoom) * 0.5f;
    }
}
