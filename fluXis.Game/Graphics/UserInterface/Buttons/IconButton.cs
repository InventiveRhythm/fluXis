using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Buttons;

public partial class IconButton : Container
{
    public float ButtonSize { get; init; } = 50;
    public Colour4 ButtonColor { get; init; } = Colour4.Transparent;
    public IconUsage Icon { get; init; } = FontAwesome6.Solid.Question;
    public float IconSize { get; init; } = 30;
    public Action Action { get; init; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private HoverLayer hover;
    private FlashLayer flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(ButtonSize);

        Child = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = ButtonColor
                },
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Icon = Icon,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(IconSize)
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();
        samples.Click();
        Action?.Invoke();
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(0.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }
}
