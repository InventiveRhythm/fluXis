using System;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class HealthBar : GameplayHUDElement
{
    private HitObjectManager manager => Screen.Playfield.Manager;
    private float health = 0;
    private Box background;
    private Box bar;

    private readonly ColourInfo drainGradient = ColourInfo.GradientHorizontal(Colour4.FromHex("#40aef8"), Colour4.FromHex("#751010"));
    private float drainRate;

    private readonly ColourInfo normalGradient = ColourInfo.GradientHorizontal(Colour4.FromHex("#ff5555"), Colour4.White);

    [BackgroundDependencyLoader]
    private void load()
    {
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
        X = Screen.Playfield.Stage.Width / 2 + 40;

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

            case HealthMode.Drain:
                //smoothen the drain rate to avoid flickering
                drainRate += (manager.HealthDrainRate - drainRate) / 5 * ((float)Clock.ElapsedFrameTime / 1000);

                float rate = Math.Clamp(drainRate, -1, 1);
                Colour4 drainColour = drainGradient.Interpolate(new Vector2((rate + 1) / 2f, 0));
                bar.Colour = drainColour;
                background.Colour = drainColour;
                break;

            default:
                float intensity = health / 100f;
                Colour4 colour = normalGradient.Interpolate(new Vector2(intensity, 0));
                bar.Colour = colour;
                background.Colour = colour;
                break;
        }

        base.Update();
    }
}
