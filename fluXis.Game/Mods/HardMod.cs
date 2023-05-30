using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class HardMod : IMod
{
    public string Name => "Hard";
    public string Acronym => "HD";
    public string Description => "Health drain, the more you miss, the faster you die.";
    public IconUsage Icon => FontAwesome.Solid.Skull;
    public float ScoreMultiplier => 1.04f;
    public bool Rankable => true;
    public IEnumerable<string> IncompatibleMods => new[] { "EZ", "NF" };
}
