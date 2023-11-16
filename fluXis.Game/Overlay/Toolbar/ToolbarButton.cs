using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarButton : Container, IHasDrawableTooltip
{
    public IconUsage Icon { get; init; } = FontAwesome.Solid.Question;
    public string Title { get; init; }
    public string Description { get; init; }

    public Action Action;

    [Resolved]
    private UISamples samples { get; set; }

    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(40);
        Margin = new MarginPadding(5);
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        CornerRadius = 5;
        Masking = true;

        Children = new Drawable[]
        {
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
            new SpriteIcon
            {
                Icon = Icon,
                Size = new Vector2(20),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        Action?.Invoke();
        samples.Click();
        return true;
    }

    public Drawable GetTooltip()
    {
        return new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Margin = new MarginPadding { Horizontal = 10, Vertical = 6 },
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new SpriteIcon
                        {
                            Icon = Icon,
                            Size = new Vector2(16),
                            Margin = new MarginPadding(4)
                        },
                        new FluXisSpriteText
                        {
                            Text = Title,
                            FontSize = 24
                        }
                    }
                },
                new FluXisTextFlow
                {
                    AutoSizeAxes = Axes.Both,
                    Text = Description,
                    FontSize = 18
                }
            }
        };
    }
}
