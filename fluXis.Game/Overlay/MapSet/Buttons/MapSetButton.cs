using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.MapSet.Buttons;

public partial class MapSetButton : Container
{
    [Resolved]
    private UISamples samples { get; set; }

    private IconUsage icon { get; }
    private Action action { get; }

    private Box hover;
    private Box flash;

    protected Container ScaleContainer { get; private set; }
    protected SpriteIcon Icon { get; private set; }

    public MapSetButton(IconUsage icon, Action action)
    {
        this.icon = icon;
        this.action = action;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(48);

        Child = ScaleContainer = new CircularContainer
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
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
                Icon = new SpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(18),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            }
        };
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        ScaleContainer.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        ScaleContainer.ScaleTo(1, 1000, Easing.OutElastic);
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

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        action?.Invoke();
        return true;
    }
}
