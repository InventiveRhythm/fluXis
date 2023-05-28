using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics;

public partial class FluXisButton : ClickableContainer
{
    public int FontSize { get; set; } = 24;
    public string Text { get; set; } = "Default Text";

    private Box hoverBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new CircularContainer
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Surface2
                },
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new SpriteText
                    {
                        Text = Text,
                        Font = FluXisFont.Default(FontSize),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        hoverBox.FadeTo(0.4f)
                .FadeTo(.2f, 400);

        return base.OnClick(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.FadeTo(0.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.FadeTo(0, 200);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        this.ScaleTo(0.95f, 4000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        this.ScaleTo(1, 800, Easing.OutElastic);
    }
}
