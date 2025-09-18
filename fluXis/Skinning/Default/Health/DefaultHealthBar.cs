using System;
using System.Diagnostics;
using fluXis.Graphics.Sprites.Text;
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

    private Colour4 top;
    private Colour4 bottom;
    private bool updateColors;

    public DefaultHealthBar(SkinJson skinJson, HealthProcessor processor)
        : base(skinJson, MapColor.Accent)
    {
        Debug.Assert(processor != null);
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
        BorderColour = ColourInfo.GradientVertical(top = GetIndexOrFallback(1, Theme.Primary), bottom = GetIndexOrFallback(2, Theme.Secondary));
        BorderThickness = 4;
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background3
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Colour = GetIndexOrFallback(1, Theme.Primary),
                FontSize = 18,
                Y = 10
            }
        };
    }

    public override void UpdateColor(MapColor index, Colour4 color)
    {
        switch (index)
        {
            case MapColor.Primary:
                top = color;
                break;

            case MapColor.Secondary:
                bottom = color;
                break;
        }

        updateColors = true;
    }

    protected override void RegisterToProvider()
    {
        ColorProvider?.Register(this, MapColor.Primary);
        ColorProvider?.Register(this, MapColor.Secondary);
    }

    protected override void UnregisterFromProvider()
    {
        ColorProvider?.Unregister(this, MapColor.Primary);
        ColorProvider?.Unregister(this, MapColor.Secondary);
    }

    protected override void Update()
    {
        base.Update();

        if (updateColors)
        {
            text.Colour = top;
            BorderColour = ColourInfo.GradientVertical(top, bottom);
            updateColors = false;
        }

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
