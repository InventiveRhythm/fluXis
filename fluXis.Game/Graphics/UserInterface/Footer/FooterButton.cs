using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.UserInterface.Footer;

public partial class FooterButton : CompositeDrawable
{
    public LocalisableString Text
    {
        get => text;
        set
        {
            text = value;

            if (SpriteText != null)
                SpriteText.Text = value;
        }
    }

    public IconUsage Icon { get; init; } = FontAwesome6.Solid.Question;
    public Action Action { get; init; }
    public Colour4 AccentColor { get; init; } = Color4.White;
    public int Index { get; init; }

    public BindableBool Enabled { get; init; } = new(true);

    [Resolved]
    private UISamples samples { get; set; }

    private Box hover;
    private Box flash;
    private Container content;
    private LocalisableString text = string.Empty;

    protected FluXisSpriteText SpriteText { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 150;
        Height = 90;
        Y = 20;
        Anchor = Origin = Anchor.BottomLeft;

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 10,
            Masking = true,
            EdgeEffect = FluXisStyles.ShadowMediumNoOffset,
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
                content = new Container
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
                        Children = new[]
                        {
                            CreateIcon(),
                            SpriteText = new FluXisSpriteText
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
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Enabled.BindValueChanged(enabled =>
        {
            content.FadeTo(enabled.NewValue ? 1 : .5f, 200, Easing.OutQuint);
        }, true);
    }

    protected virtual Drawable CreateIcon() => new SpriteIcon
    {
        Anchor = Anchor.TopCentre,
        Origin = Anchor.TopCentre,
        Size = new Vector2(24),
        Icon = Icon,
        Shadow = true,
        Colour = AccentColor
    };

    public override void Show()
    {
        this.MoveToY(100).Delay(100 * Index)
            .MoveToY(20, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!Enabled.Value)
            return false;

        flash.FadeOutFromOne(1000, Easing.OutQuint);
        Action?.Invoke();
        samples.Click();
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (!Enabled.Value)
            return false;

        hover.FadeTo(.2f, 50);
        this.MoveToY(10, 200, Easing.OutQuint);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeTo(0, 200);
        this.MoveToY(20, 400, Easing.OutQuint);
    }
}
