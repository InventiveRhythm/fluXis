using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Mods;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Modes.Keys.Mods;

public class MirrorMod : IMod, IApplicableToMap
{
    public string Name => "Mirror";
    public string Acronym => "MR";
    public string Description => "Flips the notes horizontally.";
    public IconUsage Icon => Phosphor.Bold.ArrowsHorizontal;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => 1f;
    public float RatingMultiplier => 1f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(PlayableMap map)
    {
        var count = KeysGameMode.ParseMode(map.Mode);

        foreach (var hit in map.ObjectsOfType<HitObject>())
            hit.Lane = count - hit.Lane + 1;
    }
}
