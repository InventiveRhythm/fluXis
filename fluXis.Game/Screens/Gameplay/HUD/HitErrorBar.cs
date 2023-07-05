using System;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class HitErrorBar : GameplayHUDElement
{
    private Bindable<float> scaleBind;

    private readonly SpriteIcon icon;
    private readonly Container hits;
    private readonly CircularContainer average;

    public HitErrorBar(GameplayScreen screen)
        : base(screen)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.TopCentre;
        AutoSizeAxes = Axes.Y;
        Width = 280;
        Y = 50;

        screen.Performance.OnHitStatAdded += AddHit;

        Container colors;

        InternalChildren = new Drawable[]
        {
            icon = new SpriteIcon
            {
                Icon = FontAwesome.Solid.ChevronDown,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Size = new Vector2(16)
            },
            colors = new Container
            {
                Margin = new MarginPadding { Top = 20 },
                Height = 5,
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                CornerRadius = 2.5f,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.White
                    }
                }
            },
            hits = new Container
            {
                Margin = new MarginPadding { Top = 22.5f },
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            average = new CircularContainer
            {
                Size = new Vector2(7, 7),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Margin = new MarginPadding { Top = 30 },
                Masking = true,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both
                }
            }
        };

        foreach (var hitWindow in HitWindow.LIST.Reverse())
        {
            if (hitWindow.Key == Scoring.Judgement.Miss)
                continue;

            colors.Add(new Container
            {
                RelativeSizeAxes = Axes.Y,
                Width = hitWindow.Timing * 2,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 2.5f,
                Masking = true,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = hitWindow.Color
                }
            });
        }
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        scaleBind = config.GetBindable<float>(FluXisSetting.HitErrorScale);
    }

    protected override void LoadComplete()
    {
        scaleBind.BindValueChanged(e => Scale = new Vector2(e.NewValue), true);
    }

    public void AddHit(HitStat stat)
    {
        float time = -stat.Difference;
        HitWindow hitWindow = HitWindow.FromTiming(Math.Abs(time));

        icon.MoveToX(time, 300, Easing.OutQuint);

        CircularContainer hit = new CircularContainer
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(5, 10),
            Masking = true,
            X = time,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = hitWindow.Color
            }
        };

        hits.Add(hit);
        hit.ScaleTo(0)
           .ScaleTo(1f, 200, Easing.OutQuint)
           .Then()
           .FadeOut(300)
           .Expire();

        updateAverage();
    }

    private void updateAverage()
    {
        float avg = Screen.Performance.HitStats.Average(h => h.Difference);
        HitWindow hitWindow = HitWindow.FromTiming(Math.Abs(avg));

        average.MoveToX(avg, 100, Easing.OutQuint);
        average.Colour = hitWindow.Color;
    }
}
