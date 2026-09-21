using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class NoEventMod : IMod, IApplicableToMap
{
    public string Name => "No Events";
    public string Acronym => "NEV";
    public string Description => "Removes all visual effects.";
    public IconUsage Icon => Phosphor.Bold.Diamond;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => 0.6f;
    public float RatingMultiplier => .4f;
    public bool Rankable => false;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(PlayableMap map)
    {
        var objs = map.ObjectsOfType<IMapEvent>();
        map.RemoveObjects(objs);
    }
}
