using System;
using fluXis.Graphics.Sprites;
using fluXis.Map;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class NoSvMod : IMod, IApplicableToMap, IApplicableToEvents
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

    public void Apply(MapEvents events)
    {
        events.HitObjectEaseEvents.Clear();
        events.ScrollMultiplyEvents.Clear();
        events.TimeOffsetEvents.Clear();
    }
}
