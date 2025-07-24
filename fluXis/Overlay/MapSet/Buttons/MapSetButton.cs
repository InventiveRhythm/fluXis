using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.MapSet.Buttons;

public partial class MapSetButton : Container
{
    [Resolved]
    private UISamples samples { get; set; }

    private IconUsage icon { get; }
    private Action action { get; }

    private HoverLayer hover;
    private FlashLayer flash;

    protected Container ScaleContainer { get; private set; }
    protected SpriteIcon Icon { get; private set; }

    public MapSetButton(IconUsage icon, Action action)
    {
        this.icon = icon;
        this.action = action;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(48);

        Child = ScaleContainer = new CircularContainer
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2
                },
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                Icon = new FluXisSpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(18),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            }
        };
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        ScaleContainer.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        ScaleContainer.ScaleTo(1, 1000, Easing.OutElastic);
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        flash.Show();
        action?.Invoke();
        return true;
    }
}
