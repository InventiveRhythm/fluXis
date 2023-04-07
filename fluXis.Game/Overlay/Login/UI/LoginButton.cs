using System;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.Login.UI;

public partial class LoginButton : Container
{
    private readonly Box hover;

    public Action<LoginButton> Action;

    public LoginButton(string label)
    {
        Height = 30;
        Width = 125;
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        CornerRadius = 5;
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Accent2
            },
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new SpriteText
            {
                Text = label,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = FluXisFont.Default()
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(0.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeTo(0, 200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        hover.FadeTo(.4f)
             .FadeTo(0.2f, 200);

        Action?.Invoke(this);

        return true;
    }
}
