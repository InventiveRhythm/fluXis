using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Scoring.Processing.Health;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class FragileMod : IMod, IApplicableToHealthProcessor
{
    public string Name => "Fragile";
    public string Acronym => "FR";
    public string Description => "One miss and you're dead.";
    public IconUsage Icon => FontAwesome6.Solid.WineGlassEmpty;
    public ModType Type => ModType.DifficultyIncrease;
    public float ScoreMultiplier => 1f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => new[] { typeof(FlawlessMod), typeof(AutoPlayMod), typeof(NoFailMod) };

    public void Apply(HealthProcessor processor)
        => processor.ExtraFailCondition = r => r.Judgement == Judgement.Miss;
}
