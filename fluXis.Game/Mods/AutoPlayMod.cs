using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class AutoPlayMod : IMod
{
    public string Name => "AutoPlay";
    public string Acronym => "AP";
    public string Description => "Watch a perfect playthrough of the song.";
    public IconUsage Icon => FontAwesome.Solid.Plane;
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => false;
    public string[] IncompatibleMods => new[] { "NF", "FR", "FL", "EZ" };
}
