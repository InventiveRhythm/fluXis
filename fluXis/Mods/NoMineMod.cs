using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class NoMineMod : IMod, IApplicableToMap
{
    public string Name => "No Mines";
    public string Acronym => "NMN";
    public string Description => "Removes all landmines.";
    public IconUsage Icon => FontAwesome6.Solid.Flag;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => .8f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(MapInfo map)
    {
        map.HitObjects.RemoveAll(hitObject => hitObject.Landmine);
    }
}
