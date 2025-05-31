using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Search;

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
                icon = new FluXisSpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome6.Solid.AngleDown,
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
        icon.RotateTo(rotation, 800, Easing.OutElasticQuarter);

        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.8f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 800, Easing.OutElasticHalf);
    }

    protected override bool OnHover(HoverEvent e)
    {
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
    }
}
