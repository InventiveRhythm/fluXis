using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class NoFailMod : IMod
{
    public string Name => "No Fail";
    public string Acronym => "NF";
    public string Description => "You can't fail, no matter what.";
    public IconUsage Icon => FontAwesome6.Solid.ShieldHalved;
    public ModType Type => ModType.DifficultyDecrease;
    public float ScoreMultiplier => 0.5f;
    public bool Rankable => true;
    public string[] IncompatibleMods => new[] { "EZ", "AP", "HD", "FR", "FL" };
}
