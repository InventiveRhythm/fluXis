using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class FragileMod : IMod
{
    public string Name => "Fragile";
    public string Acronym => "FR";
    public string Description => "One miss and you're dead.";
    public IconUsage Icon => FontAwesome.Solid.GlassMartini;
    public float ScoreMultiplier => 1f;
    public bool Rankable => true;
    public string[] IncompatibleMods => new[] { "FL", "HD", "AP", "EZ", "NF" };
}
