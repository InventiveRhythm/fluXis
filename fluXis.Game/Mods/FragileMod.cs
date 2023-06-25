using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class FragileMod : IMod
{
    public string Name => "Fragile";
    public string Acronym => "FR";
    public string Description => "One miss and you're dead.";
    public IconUsage Icon => FontAwesome.Solid.GlassMartini;
    public ModType Type => ModType.DifficultyIncrease;
    public float ScoreMultiplier => 1f;
    public bool Rankable => true;
    public IEnumerable<string> IncompatibleMods => new[] { "FL", "AP", "NF" };
}
