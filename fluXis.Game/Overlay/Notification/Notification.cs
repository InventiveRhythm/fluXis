using System;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Notification;

public partial class Notification : Container
{
    public float Lifetime { get; set; } = 5000;
    public bool FadeOut { get; set; } = false;

    public readonly Container Container;

    public Notification(string title, string desc, NotificationType type = NotificationType.Info)
    {
        Height = 100;
        Width = 300;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Margin = new MarginPadding { Top = 10 };

        InternalChild = Container = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Scale = new Vector2(1.1f),
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 20,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Colour = type switch
                    {
                        NotificationType.Error => ColourInfo.GradientVertical(Colour4.FromHex("#ff5555").Opacity(0), Colour4.FromHex("#ff5555")),
                        NotificationType.Info => ColourInfo.GradientVertical(FluXisColors.Accent2.Opacity(0), FluXisColors.Accent2),
                        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(10),
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Masking = true,
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Text = title,
                                    Font = new FontUsage("Quicksand", 24, "Bold")
                                },
                                new SpriteText
                                {
                                    Text = desc,
                                    Font = new FontUsage("Quicksand", 16, "Bold")
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
        Container.FadeInFromZero(100)
                 .ScaleTo(1, 200, Easing.OutQuint);

        base.LoadComplete();
    }

    protected override void Update()
    {
        Lifetime -= (float)Clock.ElapsedFrameTime;

        base.Update();
    }
}

public enum NotificationType
{
    Error,
    Info
}
