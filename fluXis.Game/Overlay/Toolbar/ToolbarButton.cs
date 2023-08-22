using System;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarButton : Container, IHasTextTooltip
{
    public IconUsage Icon = FontAwesome.Solid.Question;
    public new string Name = "";

    public string Tooltip => Name;

    public Action Action;

    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(40);
        Margin = new MarginPadding(5);
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        CornerRadius = 5;
        Masking = true;

        Children = new Drawable[]
        {
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
                Icon = Icon,
                Size = new Vector2(20),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
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
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        Action?.Invoke();
        return true;
    }
}
