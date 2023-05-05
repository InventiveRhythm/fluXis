#nullable enable
using System;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Overlay.Notification;

public partial class Notification : Container
{
    public float Lifetime { get; set; } = 5000;

    public virtual string SampleAppearing => "UI/Notifications/in.mp3";
    public virtual string SampleDisappearing => "UI/Notifications/out.mp3";

    public event Func<bool>? OnUserClick;

    public new Container Content;
    public Container Background;

    private readonly Container animationContainer;

    protected Notification()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChildren = new Drawable[]
        {
            animationContainer = new Container
            {
                CornerRadius = 10,
                Masking = true,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
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
                        Children = new Drawable[]
                        {
                            Content = new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Masking = true
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

        base.LoadComplete();
    }

    protected override void Update()
    {
        Lifetime -= (float)Clock.ElapsedFrameTime;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            OnUserClick?.Invoke();
            Lifetime = 0;
        }

        return true;
    }

    public virtual TransformSequence<Notification> PopOut() => this.FadeOut(200);
}
