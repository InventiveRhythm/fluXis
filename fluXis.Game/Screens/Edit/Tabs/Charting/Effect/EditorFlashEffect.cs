using System;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect.EffectEdit;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorFlashEvent : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    public FlashEvent FlashEvent { get; init; }

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
                Action = () => game.Overlay = new FlashEditorPanel
                {
                    Event = FlashEvent,
                    EditorClock = clock,
                    MapEvents = values.MapEvents
                }
            }
        };
    }

    protected override void Update()
    {
        if (!values.MapEvents.FlashEvents.Contains(FlashEvent))
        {
            Expire();
            return;
        }

        Y = -((FlashEvent.Time - (float)clock.CurrentTime) * values.Zoom) * 0.5f;
        line.Height = .5f * (FlashEvent.Duration * values.Zoom);
        line.Height = Math.Max(line.Height, 10);
        line.Child.Colour = Colour = ColourInfo.GradientVertical(FlashEvent.EndColor.Opacity(FlashEvent.EndOpacity), FlashEvent.StartColor.Opacity(FlashEvent.StartOpacity));
    }
}
