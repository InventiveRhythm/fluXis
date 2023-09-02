using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.UserInterface.Buttons;

public partial class FluXisButton : ClickableContainer
{
    public int FontSize { get; set; } = 24;
    public string Text { get; set; } = "Default Text";
    public Colour4 Color { get; set; } = FluXisColors.Background4;

    public ButtonData Data
    {
        set
        {
            Text = value.Text;
            Action = value.Action;
            Color = value.Color;
        }
    }

    [Resolved]
    private UISamples samples { get; set; }

    private Box hoverBox;
    private Box flashBox;
    private CircularContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = content = new CircularContainer
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
                    Colour = Color
                },
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                flashBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new FluXisSpriteText
                    {
                        Text = Text,
                        FontSize = FontSize,
                        Shadow = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        flashBox.FadeOutFromOne(1000, Easing.OutQuint);
        base.OnClick(e);
        samples.Click();
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.FadeTo(0.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.FadeTo(0, 200);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(0.95f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }
}

public class ButtonData
{
    public string Text { get; init; } = "Default Text";
    public Colour4 Color { get; init; } = FluXisColors.Background2;
    public Action Action { get; init; } = () => { };
}
