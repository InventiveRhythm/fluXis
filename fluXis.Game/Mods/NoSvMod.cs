using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class NoSvMod : IMod, IApplicableToMap
{
    public string Name => "No SV";
    public string Acronym => "NSV";
    public string Description => "Removes all scroll velocity changes.";
    public IconUsage Icon => FontAwesome6.Solid.Bars;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => .8f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(MapInfo map) => map.ScrollVelocities.ForEach(sv => sv.Multiplier = 1);
}
