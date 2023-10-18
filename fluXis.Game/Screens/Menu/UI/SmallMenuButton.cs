using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class SmallMenuButton : Container
{
    public Action Action { get; set; }
    public IconUsage Icon { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 60;

        Child = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 10,
            Masking = true,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            EdgeEffect = FluXisStyles.ShadowSmall,
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
                new SpriteIcon
                {
                    Size = new Vector2(20),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Shear = new Vector2(.2f, 0),
                    Icon = Icon
                }
            }
        };
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        Action?.Invoke();
        samples.Click();
        return true;
    }
}
