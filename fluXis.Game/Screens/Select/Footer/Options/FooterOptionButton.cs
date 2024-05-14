using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Select.Footer.Options;

public partial class FooterOptionButton : Container
{
    public LocalisableString Text { get; set; }
    public IconUsage Icon { get; set; }
    public Action Action { get; set; }
    public Colour4 Color { get; set; } = Colour4.White;

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Child = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                CornerRadius = 10,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background3
                    },
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
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.Absolute, 50),
                            new Dimension()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new SpriteIcon
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Icon = Icon,
                                    Colour = Color,
                                    Size = new Vector2(20)
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Right = 15 },
                                    Child = new TruncatingText
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Text = Text,
                                        Colour = Color
                                    }
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
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        samples.Click();
        Action?.Invoke();
        return true;
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

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }
}
