using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Scoring.Processing.Health;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Skinning.Default.Health;

public partial class DefaultHealthBar : ColorableSkinDrawable
{
    private HealthProcessor processor { get; }

    private FluXisSpriteText text;

    private readonly ColourInfo drainGradient = ColourInfo.GradientHorizontal(Colour4.FromHex("#40aef8"), Colour4.FromHex("#751010"));
    private double drainRate;

    public DefaultHealthBar(SkinJson skinJson, HealthProcessor processor)
        : base(skinJson)
    {
        this.processor = processor;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;
        CornerRadius = 10;
        Masking = true;
        BorderColour = ColourInfo.GradientVertical(GetIndexOrFallback(1, FluXisColors.Primary), GetIndexOrFallback(2, FluXisColors.Secondary));
        BorderThickness = 4;
        InternalChildren = new Drawable[]
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
                Colour = GetIndexOrFallback(1, FluXisColors.Primary),
                FontSize = 18,
                Y = 10
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        text.Text = $"{(int)Math.Round(processor.SmoothHealth)}";

        switch (processor)
        {
            case RequirementHeathProcessor requirement:
                BorderColour = text.Colour = Colour4.FromHex(requirement.RequirementReached ? "#40aef8" : "#40f840");
                break;

            case DrainHealthProcessor drain:
                //smoothen the drain rate to avoid flickering
                drainRate = Interpolation.Lerp(drainRate, drain.HealthDrainRate, Math.Exp(-0.001f * Clock.ElapsedFrameTime));
                if (!double.IsFinite(drainRate)) drainRate = 0;
                BorderColour = text.Colour = drainGradient.Interpolate(new Vector2(1 - (Math.Clamp((float)drainRate, -.2f, .2f) + .2f) / .4f, 0));
                break;
        }
    }
}
