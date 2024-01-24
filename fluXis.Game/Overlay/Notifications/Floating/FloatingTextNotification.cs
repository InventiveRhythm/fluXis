using System;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Notifications.Floating;

public partial class FloatingTextNotification : FloatingNotification
{
    private const int border_thickness = 2;

    public string Text { get; set; }
    public string SubText { get; set; }
    public IconUsage Icon { get; set; } = FontAwesome6.Solid.Info;
    public Color4 AccentColor { get; set; } = FluXisColors.Highlight;
    public float Lifetime { get; set; } = 5000;
    public Action Action { get; set; }

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
            Height = 50,
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
                new FillFlowContainer
                {
                    Width = 50,
                    AutoSizeAxes = Axes.X,
                    AutoSizeDuration = 400,
                    AutoSizeEasing = Easing.OutQuint,
                    RelativeSizeAxes = Axes.Y,
                    Padding = new MarginPadding(5),
                    Direction = FillDirection.Horizontal,
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
                                new SpriteIcon
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
                            Padding = new MarginPadding { Horizontal = 10 },
                            Alpha = 0,
                            Children = new Drawable[]
                            {
                                new FluXisSpriteText
                                {
                                    Text = Text
                                },
                                new FluXisSpriteText
                                {
                                    Text = SubText,
                                    FontSize = 14
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

        animationContainer.MoveToY(-70).MoveToY(0, 400, Easing.OutQuint);
        textContainer.Delay(900).FadeIn();

        this.ResizeHeightTo(50, 400, Easing.OutQuint).FadeInFromZero(400)
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
        this.FadeOut(400).OnComplete(_ =>
        {
            disappear?.Play();
            this.Delay(disappear?.Length ?? 0).Expire();
        });
    }
}
