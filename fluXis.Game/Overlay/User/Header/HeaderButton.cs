using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.User.Header;

public partial class HeaderButton : CircularContainer
{
    public IconUsage Icon { get; set; }
    public string Text { get; set; }
    public Action Action { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        Height = 48;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowSmall;

        InternalChildren = new Drawable[]
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
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                Alpha = Action != null ? 1 : .5f,
                Padding = new MarginPadding
                {
                    Horizontal = Text != null ? 20 : 14
                },
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Icon = Icon,
                        Size = new Vector2(20)
                    },
                    new FluXisSpriteText
                    {
                        Text = Text,
                        Alpha = Text != null ? 1 : 0,
                        WebFontSize = 16
                    }
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        if (Action == null) return false;

        hover.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click(Action == null);

        if (Action == null) return false;

        flash.FadeOutFromOne(1000, Easing.OutQuint);
        Action?.Invoke();
        return true;
    }
}
