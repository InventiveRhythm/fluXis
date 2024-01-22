using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class FlawlessMod : IMod
{
    public string Name => "Flawless";
    public string Acronym => "FL";
    public string Description => "Only the best will do.";
    public IconUsage Icon => FontAwesome6.Solid.ThumbsUp;
    public ModType Type => ModType.DifficultyIncrease;
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => true;
    public string[] IncompatibleMods => new[] { "NF", "AP", "FR" };
}
