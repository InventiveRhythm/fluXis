using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class NoSvMod : IMod, IApplicableToMap
{
    public string Name => "No SV";
    public string Acronym => "NSV";
    public string Description => "Removes all scroll velocity changes.";
    public IconUsage Icon => Phosphor.Bold.List;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => .8f;
    public float RatingMultiplier => .6f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    // TODO: move to keys mode
    public void Apply(PlayableMap map)
    {
        var objs = map.ObjectsOfType<IScrollEvent>();
        map.RemoveObjects(objs);
    }
}
