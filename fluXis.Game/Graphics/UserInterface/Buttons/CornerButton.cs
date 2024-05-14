using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Screens;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Buttons;

public partial class CornerButton : Container
{
    private const int corner_radius = 20;

    private LocalisableString buttonText;

    public virtual LocalisableString ButtonText
    {
        get => buttonText;
        set
        {
            buttonText = value;

            if (text != null)
                text.Text = value;
        }
    }

    public virtual IconUsage Icon { get; set; } = FontAwesome6.Solid.Question;
    public virtual Colour4 ButtonColor { get; set; } = FluXisColors.Background4;
    public Corner Corner { get; set; } = Corner.BottomLeft;
    public Action Action { get; set; }
    public bool ShowImmediately { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Box hover;
    private Box flash;

    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 250 + corner_radius;
        Height = 80 + corner_radius;
        CornerRadius = corner_radius;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowMediumNoOffset;

        Anchor = Origin = Corner switch
        {
            Corner.TopLeft => Anchor.TopLeft,
            Corner.TopRight => Anchor.TopRight,
            Corner.BottomLeft => Anchor.BottomLeft,
            Corner.BottomRight => Anchor.BottomRight,
            _ => Anchor
        };

        Shear = new Vector2(Corner is Corner.BottomLeft or Corner.TopLeft ? -.1f : .1f, 0);
        Position = new Vector2(Corner is Corner.BottomLeft or Corner.TopLeft ? -200 : 200, Corner is Corner.TopLeft or Corner.TopRight ? -corner_radius : corner_radius);

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
                Shear = new Vector2(-Shear.X, 0),
                Padding = new MarginPadding
                {
                    Left = Corner is Corner.BottomLeft or Corner.TopLeft ? corner_radius : 0,
                    Right = Corner is Corner.BottomRight or Corner.TopRight ? corner_radius : 0,
                    Bottom = Corner is Corner.BottomLeft or Corner.BottomRight ? corner_radius : 0,
                    Top = Corner is Corner.TopLeft or Corner.TopRight ? corner_radius : 0
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
                        text = new FluXisSpriteText
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

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (ShowImmediately)
            Show();
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(500);
        Action?.Invoke();
        samples.Click();
        return true;
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

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        this.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        this.ScaleTo(1, 1000, Easing.OutElastic);
    }

    public override void Show()
    {
        var x = Corner switch
        {
            Corner.BottomLeft or Corner.TopLeft => -corner_radius,
            Corner.BottomRight or Corner.TopRight => corner_radius,
            _ => 0
        };

        this.MoveToX(x, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    }

    public override void Hide()
    {
        var x = Corner switch
        {
            Corner.BottomLeft or Corner.TopLeft => -200,
            Corner.BottomRight or Corner.TopRight => 200,
            _ => 0
        };

        this.MoveToX(x, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    }
}
