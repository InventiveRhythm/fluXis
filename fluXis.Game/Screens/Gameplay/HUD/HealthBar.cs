using System;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.Ruleset;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class HealthBar : GameplayHUDElement
{
    private readonly HitObjectManager manager;
    private float health;
    private readonly Box background;
    private readonly Box bar;

    private readonly ColourInfo drainGradient = ColourInfo.GradientHorizontal(Colour4.FromHex("#40aef8"), Colour4.FromHex("#751010"));
    private float drainRate;

    public HealthBar(GameplayScreen screen)
        : base(screen)
    {
        manager = screen.Playfield.Manager;
        health = 0;

        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        CornerRadius = 10;
        Masking = true;
        Y = -40;
        Width = 40;
        Height = 500;

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = .2f,
                Blending = BlendingParameters.Additive
            },
            bar = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft
            }
        };
    }

    protected override void Update()
    {
        X = Screen.Playfield.Stage.Background.Width / 2 + 40;

        if (manager.Health != health)
            this.TransformTo(nameof(health), manager.Health, 300, Easing.OutQuint);

        bar.Height = health / 100;

        switch (manager.HealthMode)
        {
            case HealthMode.Requirement:
                Colour4 reqColour = Colour4.FromHex(manager.Health >= 70 ? "#40aef8" : "#40f840");
                bar.Colour = reqColour;
                background.Colour = reqColour;
                break;

            case HealthMode.Normal:
                float intensity = health / 100;
                Colour4 colour = Colour4.FromHSL(0, .7f * (1 - intensity), 1);
                bar.Colour = colour;
                background.Colour = colour;
                break;

            case HealthMode.Drain:
                //smoothen the drain rate to avoid flickering
                drainRate += (manager.HealthDrainRate - drainRate) / 5 * ((float)Clock.ElapsedFrameTime / 1000);

                float rate = Math.Clamp(drainRate, -1, 1);
                Colour4 drainColour = drainGradient.Interpolate(new Vector2((rate + 1) / 2f, 0));
                bar.Colour = drainColour;
                background.Colour = drainColour;
                break;

            default:
                bar.Colour = Colour4.White;
                background.Colour = Colour4.White;
                break;
        }

        base.Update();
    }
}
