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

public partial class FooterCornerButton : Container
{
    private const int corner_radius = 20;

    public Action Action { get; set; }

    protected virtual string ButtonText => "Button";
    protected virtual IconUsage Icon => FontAwesome.Solid.Question;
    protected virtual Colour4 ButtonColor => FluXisColors.Background4;

    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        bool isRight = Anchor == Anchor.BottomRight;

        Width = 250 + corner_radius;
        Height = 80 + corner_radius;
        Position = new Vector2(isRight ? 200 : -200, corner_radius);
        Shear = new Vector2(isRight ? .1f : -.1f, 0);
        CornerRadius = corner_radius;
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
                Colour = ButtonColor
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
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(isRight ? -.1f : .1f, 0),
                Padding = new MarginPadding
                {
                    Left = isRight ? 0 : corner_radius,
                    Right = isRight ? corner_radius : 0,
                    Bottom = corner_radius
                },
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(10),
                    Children = new Drawable[]
                    {
                        new SpriteIcon
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Icon = Icon,
                            Size = new Vector2(24)
                        },
                        new FluXisSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            FontSize = 24,
                            Text = ButtonText
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(500);
        Action?.Invoke();
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        this.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        this.ScaleTo(1, 1000, Easing.OutElastic);
    }
}

