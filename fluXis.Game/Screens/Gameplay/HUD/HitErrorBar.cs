using System;
using System.Linq;
using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class HitErrorBar : GameplayHUDElement
{
    private readonly SpriteIcon icon;
    private readonly Container hits;

    public HitErrorBar(GameplayScreen screen)
        : base(screen)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.TopCentre;
        Margin = new MarginPadding { Top = 50 };
        AutoSizeAxes = Axes.Y;
        Width = 280;

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
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 2.5f,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.White,
                    },
                }
            },
            hits = new Container
            {
                Margin = new MarginPadding { Top = 20 },
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }
        };

        foreach (var hitWindow in HitWindow.LIST.Reverse())
        {
            if (hitWindow.Key == Judgements.Miss)
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
                    Colour = hitWindow.Color,
                }
            });
        }
    }

    public void AddHit(float time)
    {
        HitWindow hitWindow = HitWindow.FromTiming(Math.Abs(time));

        icon.MoveToX(time, 100, Easing.OutQuint);

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
                Colour = hitWindow.Color,
            }
        };

        hits.Add(hit);
        hit.ScaleTo(0)
           .ScaleTo(1f, 200, Easing.OutQuint)
           .Then()
           .FadeOut(300);
    }
}
