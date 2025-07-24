using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Graphics.UserInterface.Files.Entry;

public abstract partial class GenericEntry : Container
{
    protected abstract IconUsage Icon { get; }
    public abstract string Text { get; }
    protected abstract string SubText { get; }
    protected abstract Colour4 Color { get; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container contentWrapper { get; set; }

    private float contentPadding
    {
        get => contentWrapper.Padding.Left;
        set => contentWrapper.Padding = new MarginPadding { Left = value };
    }

    private Box background { get; set; }
    private HoverLayer hover { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;
        CornerRadius = 5;
        Masking = true;
        Children = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color,
                Alpha = 0
            },
            contentWrapper = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 5,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Theme.Background3
                        },
                        hover = new HoverLayer { Colour = Color },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding(10),
                            Children = new Drawable[]
                            {
                                new FluXisSpriteIcon
                                {
                                    Icon = Icon,
                                    Colour = Color,
                                    Size = new Vector2(20),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft
                                },
                                new FluXisSpriteText
                                {
                                    Text = Text,
                                    Colour = Color,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    X = 30
                                },
                                new FluXisSpriteText
                                {
                                    Text = SubText,
                                    Colour = Color,
                                    Alpha = .8f,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        return true;
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

    protected void SetSelected(bool selected)
    {
        if (selected)
        {
            this.TransformTo(nameof(contentPadding), 10f, 400, Easing.OutQuint);
            background.FadeIn();
        }
        else
        {
            this.TransformTo(nameof(contentPadding), 0f, 400, Easing.OutQuint).OnComplete(_ =>
            {
                background.FadeOut(200);
            });
        }
    }
}
