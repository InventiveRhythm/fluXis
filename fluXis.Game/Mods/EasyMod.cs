using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class EasyMod : IMod
{
    public string Name => "Easy";
    public string Acronym => "EZ";
    public string Description => "Health starts at 0, reach 70% to pass.";
    public IconUsage Icon => FontAwesome.Solid.CandyCane;
    public float ScoreMultiplier => 0.7f;
    public bool Rankable => true;
    public string[] IncompatibleMods => new[] { "NF", "HD", "FR", "FL", "AP" };
}
