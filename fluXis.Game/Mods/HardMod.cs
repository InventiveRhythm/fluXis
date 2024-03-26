using System;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class HardMod : IMod
{
    public string Name => "Hard";
    public string Acronym => "HD";
    public string Description => "Health drain, the more you miss, the faster you die.";
    public IconUsage Icon => FontAwesome6.Solid.Skull;
    public ModType Type => ModType.DifficultyIncrease;
    public float ScoreMultiplier => 1.04f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => new[] { typeof(EasyMod), typeof(NoFailMod) };
}
