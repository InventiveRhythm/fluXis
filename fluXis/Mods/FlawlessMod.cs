using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Processing.Health;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class FlawlessMod : IMod, IApplicableToHealthProcessor
{
    public string Name => "Flawless";
    public string Acronym => "FL";
    public string Description => "Only the best will do.";
    public IconUsage Icon => FontAwesome6.Solid.ThumbsUp;
    public ModType Type => ModType.DifficultyIncrease;
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => new[] { typeof(NoFailMod), typeof(AutoPlayMod), typeof(FragileMod) };

    public void Apply(HealthProcessor processor)
        => processor.ExtraFailCondition = r => r.Judgement < Judgement.Flawless;
}
