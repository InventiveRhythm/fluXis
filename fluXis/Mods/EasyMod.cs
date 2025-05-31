using System;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class EasyMod : IMod
{
    public string Name => "Easy";
    public string Acronym => "EZ";
    public string Description => $"Health starts at 0, reach {HEALTH_REQUIREMENT}% to pass.";
    public IconUsage Icon => FontAwesome6.Solid.CandyCane;
    public ModType Type => ModType.DifficultyDecrease;
    public float ScoreMultiplier => 0.7f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => new[] { typeof(HardMod), typeof(NoFailMod) };

    public const float HEALTH_REQUIREMENT = 80f;
}
