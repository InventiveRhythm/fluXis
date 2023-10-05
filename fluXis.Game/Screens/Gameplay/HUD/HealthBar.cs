using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class HealthBar : GameplayHUDElement
{
    private HitObjectManager manager => Screen.Playfield.Manager;
    private float health = 0;
    private Container bar;
    private FluXisSpriteText text;
    private SpriteIcon icon;

    private bool showingIcon;

    private readonly ColourInfo drainGradient = ColourInfo.GradientHorizontal(Colour4.FromHex("#40aef8"), Colour4.FromHex("#751010"));
    private double drainRate;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomLeft;
        Width = 40;
        Height = 500;
        Y = -40;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 10,
                Masking = true,
                BorderColour = Colour4.Black.Opacity(.8f),
                BorderThickness = 4,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = 0.5f
                    }
                }
            },
            bar = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                CornerRadius = 10,
                Masking = true,
                BorderColour = ColourInfo.GradientVertical(FluXisColors.Accent, FluXisColors.Accent4),
                BorderThickness = 4,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background3
                    },
                    text = new FluXisSpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Colour = FluXisColors.Accent,
                        FontSize = 18,
                        Y = 10
                    }
                }
            },
            icon = new SpriteIcon
            {
                Size = new Vector2(30),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = FontAwesome.Solid.Times,
                Alpha = 0
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Screen.HealthProcessor.Health.BindValueChanged(e => this.TransformTo(nameof(health), e.NewValue, 300, Easing.OutQuint), true);
    }

    protected override void Update()
    {
        X = Screen.Playfield.Stage.Width / 2 + 20 + Screen.Playfield.X;

        if (!showingIcon && Screen.HealthProcessor.FailedAlready)
        {
            showingIcon = true;
            icon.FadeIn(600).Then(400).FadeOut(600).Loop();
        }

        bar.Height = health / 100;
        text.Text = $"{Math.Floor(health)}";

        switch (Screen.HealthProcessor)
        {
            case RequirementHeathProcessor requirement:
                bar.BorderColour = text.Colour = Colour4.FromHex(requirement.RequirementReached ? "#40aef8" : "#40f840");
                break;

            case DrainHealthProcessor drain:
                //smoothen the drain rate to avoid flickering
                drainRate = Interpolation.Lerp(drainRate, drain.HealthDrainRate, Math.Exp(-0.001f * Clock.ElapsedFrameTime));
                bar.BorderColour = text.Colour = drainGradient.Interpolate(new Vector2((Math.Clamp((float)drainRate, -1, 1) + 1) / 2f, 0));
                break;
        }

        base.Update();
    }
}
