using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Search;

public partial class DropdownIcon : Container
{
    public Action Action;

    private readonly Box background;
    private readonly Container content;
    private readonly SpriteIcon icon;

    private int rotation;

    public DropdownIcon()
    {
        Height = 30;
        Width = 30;
        Anchor = Origin = Anchor.CentreRight;
        Margin = new MarginPadding { Right = 5 };

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.ChevronDown,
                    Size = new Vector2(20)
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();

        background.FadeTo(.4f)
                  .FadeTo(.2f, 400);

        rotation += 180;
        icon.RotateTo(rotation, 400, Easing.OutQuint);

        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 4000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 800, Easing.OutElastic);
    }

    protected override bool OnHover(HoverEvent e)
    {
        background.FadeTo(.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        background.FadeTo(0, 200);
    }
}
