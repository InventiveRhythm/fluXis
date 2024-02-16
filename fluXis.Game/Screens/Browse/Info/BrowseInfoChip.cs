using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Browse.Info;

public partial class BrowseInfoChip : Container
{
    public string Title { get; set; } = "Default Title";
    public string DefaultText { get; set; } = "Default Text";
    public Action OnClickAction { get; set; }

    public string Text
    {
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                text.Text = DefaultText;
                text.Alpha = 0.5f;
            }
            else
            {
                text.Text = value;
                text.Alpha = 1;
            }
        }
    }

    [Resolved]
    private UISamples samples { get; set; }

    private Box hover;
    private Box flash;
    private TruncatingText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 375;
        Height = 60;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
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
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(15) { Vertical = 12 },
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new TruncatingText
                    {
                        RelativeSizeAxes = Axes.X,
                        Text = Title,
                        FontSize = 16,
                        Colour = FluXisColors.Text2
                    },
                    text = new TruncatingText
                    {
                        RelativeSizeAxes = Axes.X,
                        Text = DefaultText,
                        Alpha = 0.5f,
                        FontSize = 20
                    }
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (OnClickAction == null) return false;

        samples.Hover();
        hover.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (OnClickAction == null) return false;

        samples.Click();
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        OnClickAction.Invoke();
        return true;
    }
}
