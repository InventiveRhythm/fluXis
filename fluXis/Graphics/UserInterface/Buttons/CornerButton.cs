using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens;
using fluXis.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface.Buttons;

public partial class CornerButton : Container
{
    private const int corner_radius = 20;

    private IconUsage iconSprite = FontAwesome6.Solid.Question;
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

    public virtual IconUsage Icon
    {
        get => iconSprite;
        set
        {
            iconSprite = value;

            if (icon != null)
                icon.Icon = value;
        }
    }

    public virtual Colour4 ButtonColor { get; set; } = FluXisColors.Background4;
    public Corner Corner { get; set; } = Corner.BottomLeft;
    public Action Action { get; set; }
    public bool ShowImmediately { get; set; }
    public bool PlayClickSound { get; set; } = true;

    [Resolved]
    private UISamples samples { get; set; }

    private HoverLayer hover;
    private FlashLayer flash;

    private SpriteIcon icon;
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

        Shear = new Vector2(Corner is Corner.BottomLeft or Corner.TopRight ? -.1f : .1f, 0);
        Position = new Vector2(Corner is Corner.BottomLeft or Corner.TopLeft ? -200 : 200, Corner is Corner.TopLeft or Corner.TopRight ? -corner_radius : corner_radius);

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ButtonColor
            },
            hover = new HoverLayer(),
            flash = new FlashLayer(),
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
                    Spacing = new Vector2(8),
                    Children = new Drawable[]
                    {
                        icon = new FluXisSpriteIcon
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Icon = Icon,
                            Size = new Vector2(16)
                        },
                        text = new FluXisSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            WebFontSize = 16,
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

        if (PlayClickSound)
            samples.Click();

        Action?.Invoke();
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
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
