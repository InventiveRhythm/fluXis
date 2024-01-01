using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class AutoPlayV2Mod : IMod
{
    public string Name => "AutoPlay V2";
    public string Acronym => "AP2";
    public string Description => "Experimental AutoPlay which uses replay data.";
    public IconUsage Icon => FontAwesome.Solid.Plane;
    public ModType Type => ModType.Automation;
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => false;
    public string[] IncompatibleMods => new[] { "NF", "FR", "FL", "AP" };
}
