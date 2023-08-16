using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooterButton : Container
{
    public string Text
    {
        get => text;
        set
        {
            text = value;

            if (spriteText != null)
                spriteText.Text = value;
        }
    }

    public IconUsage Icon { get; init; } = FontAwesome.Solid.Question;
    public Action Action { get; set; }
    public Colour4 AccentColor { get; init; } = Color4.White;
    public int Index { get; init; }

    private Box hover;
    private Box flash;
    private FluXisSpriteText spriteText;
    private string text = string.Empty;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 150;
        Height = 90;
        Y = 20;
        Anchor = Origin = Anchor.BottomLeft;
        CornerRadius = 10;
        Masking = true;
        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Color4.Black.Opacity(.25f),
            Radius = 10
        };

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
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Bottom = 10 },
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(0, 5),
                    Children = new Drawable[]
                    {
                        new SpriteIcon
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Size = new Vector2(24),
                            Icon = Icon,
                            Shadow = true,
                            Colour = AccentColor
                        },
                        spriteText = new FluXisSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = Text,
                            Shadow = true
                        }
                    }
                }
            },
            flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Colour = AccentColor
            }
        };
    }

    public override void Show()
    {
        this.MoveToY(100).Delay(100 * Index).MoveToY(20, 500, Easing.OutQuint);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        Action?.Invoke();
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        this.MoveToY(10, 200, Easing.OutQuint);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeTo(0, 200);
        this.MoveToY(20, 400, Easing.OutQuint);
    }
}
