using System;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Compose.Effect.EffectEdit;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Effect;

public partial class EditorFlashEvent : Container, IHasPopover
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    public FlashEvent FlashEvent { get; init; }

    private FlashEffectEditor flashEffectEditor;
    private CircularContainer line;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        Children = new Drawable[]
        {
            new ClickableContainer
            {
                AutoSizeAxes = Axes.Both,
                Child = line = new CircularContainer
                {
                    Masking = true,
                    Width = 10,
                    BorderColour = Colour4.White,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientVertical(FlashEvent.EndColor.Opacity(FlashEvent.EndOpacity), FlashEvent.StartColor.Opacity(FlashEvent.StartOpacity))
                    }
                },
                Action = this.ShowPopover
            }
        };
    }

    protected override void Update()
    {
        Y = -((FlashEvent.Time - (float)clock.CurrentTime) * values.Zoom) * 0.5f;
        line.Height = .5f * (FlashEvent.Duration * values.Zoom);
        line.Height = Math.Max(line.Height, 10);
        line.Child.Colour = Colour = ColourInfo.GradientVertical(FlashEvent.EndColor.Opacity(FlashEvent.EndOpacity), FlashEvent.StartColor.Opacity(FlashEvent.StartOpacity));
    }

    public Popover GetPopover() => flashEffectEditor = new FlashEffectEditor { FlashEvent = FlashEvent };
}
