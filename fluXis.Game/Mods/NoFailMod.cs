using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Scoring.Processing.Health;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class NoFailMod : IMod, IApplicableToHealthProcessor
{
    public string Name => "No Fail";
    public string Acronym => "NF";
    public string Description => "You can't fail, no matter what.";
    public IconUsage Icon => FontAwesome6.Solid.ShieldHalved;
    public ModType Type => ModType.DifficultyDecrease;
    public float ScoreMultiplier => 0.5f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => new[] { typeof(EasyMod), typeof(AutoPlayMod), typeof(HardMod), typeof(FragileMod), typeof(FlawlessMod) };

    public void Apply(HealthProcessor processor) => processor.CanFail = false;
}
