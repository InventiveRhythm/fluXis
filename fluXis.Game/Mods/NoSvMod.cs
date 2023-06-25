using System;
using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class NoSvMod : IMod
{
    public string Name => "No SV";
    public string Acronym => "NSV";
    public string Description => "Removes all scroll velocity changes.";
    public IconUsage Icon => FontAwesome.Solid.Bars;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => .8f;
    public bool Rankable => true;
    public IEnumerable<string> IncompatibleMods => Array.Empty<string>();
}
