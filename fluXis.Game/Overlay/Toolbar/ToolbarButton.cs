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

    private Box background;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(30);
        Margin = new MarginPadding(5);

        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Shear = new Vector2(0.1f, 0),
                Child = background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                }
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
        background.FadeTo(.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        background.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        return true;
    }
}
