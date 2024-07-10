using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public partial class PointSettingsToCurrentButton : Container, IHasTooltip
{
    public LocalisableString TooltipText => "Move to current time.";

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public Action<double> Action { get; init; }

    private Container content;
    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(32);

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            CornerRadius = 5,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background3
                },
                hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                flash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new SpriteIcon
                {
                    Size = new Vector2(16),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome6.Solid.Clock
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        Action?.Invoke(clock.CurrentTime);
        return base.OnClick(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(0.95f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 800, Easing.OutElasticHalf);
    }
}
