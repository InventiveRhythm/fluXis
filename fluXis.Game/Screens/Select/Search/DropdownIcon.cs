using System;
using fluXis.Game.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Search;

public partial class DropdownIcon : Container
{
    public Action Action;

    [Resolved]
    private UISamples samples { get; set; }

    private readonly Container content;
    private readonly SpriteIcon icon;

    private int rotation;

    public DropdownIcon()
    {
        Size = new Vector2(30);
        Anchor = Origin = Anchor.Centre;

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.ChevronDown,
                    Size = new Vector2(24)
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        samples.Click();

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
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
    }
}
