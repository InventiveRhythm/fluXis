using System;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Overlay.Notifications.Floating;

public partial class FloatingTextNotification : FloatingNotification
{
    private const int border_thickness = 2;

    public string Text { get; init; }
    public string SubText { get; init; }
    public IconUsage Icon { get; init; } = FontAwesome6.Solid.Info;
    public Color4 AccentColor { get; init; } = FluXisColors.Highlight;
    public float Lifetime { get; init; } = 5000;
    public Action Action { get; init; }

    public string SampleAppearing { get; set; } = "UI/Notifications/in.mp3";
    public string SampleDisappearing { get; set; } = "UI/Notifications/out.mp3";

    private Container animationContainer;
    private FillFlowContainer textContainer;

    private Sample appear;
    private Sample disappear;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        appear = samples.Get(SampleAppearing);
        disappear = samples.Get(SampleDisappearing);

        AutoSizeAxes = Axes.X;
        Child = animationContainer = new Container
        {
            AutoSizeAxes = Axes.X,
            Height = 48,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Children = new Drawable[]
            {
                new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    EdgeEffect = FluXisStyles.Glow(AccentColor, 10),
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    }
                },
                new Container
                {
                    Width = 48,
                    AutoSizeAxes = Axes.X,
                    AutoSizeDuration = 600,
                    AutoSizeEasing = Easing.InOutCubic,
                    RelativeSizeAxes = Axes.Y,
                    Padding = new MarginPadding(4),
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new CircularContainer
                        {
                            Size = new Vector2(40),
                            Masking = true,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = AccentColor
                                },
                                new FluXisSpriteIcon
                                {
                                    Icon = Icon,
                                    Size = new Vector2(20),
                                    Colour = FluXisColors.Background2,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                }
                            }
                        },
                        textContainer = new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding { Left = 48, Right = 12 },
                            Spacing = new Vector2(-2),
                            Alpha = 0,
                            Children = new Drawable[]
                            {
                                new FluXisSpriteText
                                {
                                    Text = Text,
                                    WebFontSize = 14
                                },
                                new FluXisSpriteText
                                {
                                    Text = SubText,
                                    WebFontSize = 10
                                }
                            }
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(-border_thickness + 1),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = new CircularContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        BorderThickness = border_thickness,
                        BorderColour = AccentColor,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Transparent
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        appear?.Play();

        animationContainer.MoveToY(-70).MoveToY(0, 600, Easing.OutQuint);
        textContainer.Delay(600).FadeIn();

        this.ResizeHeightTo(48, 600, Easing.OutQuint).FadeInFromZero(300)
            .Then(Lifetime).FadeIn().OnComplete(_ => remove());
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        remove();
        return true;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        appear?.Dispose();
        disappear?.Dispose();
    }

    private void remove()
    {
        this.FadeOut(300).OnComplete(_ =>
        {
            disappear?.Play();
            this.Delay(disappear?.Length ?? 0).Expire();
        });
    }
}
