#nullable enable
using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Overlay.Notification;

public partial class Notification : Container
{
    public float Lifetime { get; set; } = 5000;

    public virtual string SampleAppearing => "UI/Notifications/in.mp3";
    public virtual string SampleDisappearing => "UI/Notifications/out.mp3";
    public virtual bool ShowCloseButton => true;

    public event Func<bool>? OnUserClick;

    protected new Container Content { get; set; }
    protected Container IconContainer { get; set; }
    protected Container Background { get; set; }
    protected CloseIcon CloseButton { get; set; }

    private readonly Container animationContainer;

    protected Notification()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChildren = new Drawable[]
        {
            animationContainer = new Container
            {
                CornerRadius = 20,
                Masking = true,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Colour4.Black.Opacity(.25f),
                    Radius = 10
                },
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Surface2
                    },
                    Background = new Container
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(10),
                        Child = new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize, minSize: 40)
                            },
                            ColumnDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(),
                                new Dimension(GridSizeMode.AutoSize)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    IconContainer = new Container
                                    {
                                        RelativeSizeAxes = Axes.Y,
                                        Width = 40
                                    },
                                    Content = new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Masking = true
                                    },
                                    CloseButton = new CloseIcon
                                    {
                                        Alpha = ShowCloseButton ? .4f : 0,
                                        CloseAction = () => Lifetime = 0
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        this.FadeInFromZero(200);

        animationContainer.MoveToX(DrawSize.X)
                          .MoveToX(0, 400, Easing.OutQuint);
    }

    protected override void Update()
    {
        Lifetime -= (float)Clock.ElapsedFrameTime;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            if (OnUserClick != null)
            {
                OnUserClick.Invoke();
                Lifetime = 0;
            }
        }

        return true;
    }

    public virtual TransformSequence<Notification> PopOut() => this.FadeOut(200);

    protected partial class CloseIcon : SpriteIcon
    {
        public Action? CloseAction { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Margin = new MarginPadding { Horizontal = 10 };
            Size = new Vector2(20);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Icon = FontAwesome.Solid.Times;
        }

        protected override bool OnClick(ClickEvent e)
        {
            CloseAction?.Invoke();
            return true;
        }

        protected override bool OnHover(HoverEvent e)
        {
            this.FadeIn(200);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.FadeTo(.4f, 200);
        }
    }
}
