using System;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class MenuButton : Container
{
    public Action Action { get; set; }
    public string Text { set => text.Text = value; }
    public string Description { set => description.Text = value; }
    public IconUsage Icon { set => icon.Icon = value; }

    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            Alpha = value ? 1 : .5f;
        }
    }

    private bool enabled = true;

    private readonly Box hover;
    private readonly SpriteIcon icon;
    private readonly SpriteText text;
    private readonly SpriteText description;

    public MenuButton()
    {
        Height = 60;
        CornerRadius = 10;
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
            },
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
            },
            icon = new SpriteIcon
            {
                Size = new Vector2(30),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 10 },
            },
            text = new SpriteText
            {
                Font = new FontUsage("Quicksand", 30, "Bold"),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Left = 50 },
                Y = 6
            },
            description = new SpriteText
            {
                Font = new FontUsage("Quicksand"),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.TopLeft,
                Margin = new MarginPadding { Left = 50 },
                Y = -1
            },
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (!Enabled)
            return false;

        hover.FadeTo(.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (!Enabled)
            return;

        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!Enabled)
            return false;

        Action?.Invoke();

        return true;
    }
}
