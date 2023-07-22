using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooterButton : Container
{
    public string Text { get; init; } = string.Empty;
    public Action Action { get; set; }

    private Box hoverBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;

        InternalChildren = new Drawable[]
        {
            hoverBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Margin = new MarginPadding { Horizontal = 20 },
                FontSize = 24,
                Text = Text
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        hoverBox.FadeTo(.4f).FadeTo(.2f, 200);
        Action?.Invoke();
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.FadeTo(0, 200);
        base.OnHoverLost(e);
    }
}
