using System;
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
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Select.Footer.Options;

public partial class FooterOptionButton : Container
{
    public LocalisableString Text { get; set; }
    public IconUsage Icon { get; set; }
    public Action Action { get; set; }
    public Colour4 Color { get; set; } = FluXisColors.Text;

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private HoverLayer hover;
    private FlashLayer flash;

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
                    hover = new HoverLayer { Colour = Color },
                    flash = new FlashLayer { Colour = Color },
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
                                new FluXisSpriteIcon
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
        flash.Show();
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
        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }
}
