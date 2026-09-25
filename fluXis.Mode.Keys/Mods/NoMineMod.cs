using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map;
using fluXis.Mode.Keys.Objects;
using fluXis.Mods;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mode.Keys.Mods;

public class NoMineMod : IMod, IApplicableToMap
{
    public string Name => "No Mines";
    public string Acronym => "NMN";
    public string Description => "Removes all landmines.";
    public IconUsage Icon => Phosphor.Bold.Flag;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => .8f;
    public float RatingMultiplier => .6f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(PlayableMap map)
        => map.RemoveObjects(map.ObjectsOfType<Landmine>());
}
