using System;
using fluXis.Game.Graphics;
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
    public string Text { set => text.Text = value; }
    public IconUsage Icon { set => icon.Icon = value; }

    private readonly Box hover;
    private readonly SpriteIcon icon;
    private readonly SpriteText text;

    public SmallMenuButton()
    {
        Height = 40;
        CornerRadius = 5;
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background
            },
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            icon = new SpriteIcon
            {
                Size = new Vector2(20),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 10 }
            },
            text = new SpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 40 },
                Font = new FontUsage("Quicksand", 20, "Bold"),
                Y = -2
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        return true;
    }
}
