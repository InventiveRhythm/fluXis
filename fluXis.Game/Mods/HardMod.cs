using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class HardMod : IMod
{
    public string Name => "Hard";
    public string Acronym => "HD";
    public string Description => "Health drain, the more you miss, the faster you die.";
    public IconUsage Icon => FontAwesome.Solid.Skull;
    public float ScoreMultiplier => 1.12f;
    public bool Rankable => true;
    public string[] IncompatibleMods => new[] { "NF", "EZ", "FL", "FR" };
}
