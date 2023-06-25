using System;
using System.Collections.Generic;
using fluXis.Game.Utils;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class RateMod : IMod
{
    public string Name => "Rate";
    public string Description => "Change the rate of the map";
    public IconUsage Icon => FontAwesome.Solid.Clock;
    public ModType Type => ModType.Rate;
    public bool Rankable => true;
    public IEnumerable<string> IncompatibleMods => Array.Empty<string>();

    public string Acronym => $"{Rate.ToStringInvariant()}x";
    public float ScoreMultiplier => 1f + (Rate - 1f) * 0.4f;
    public float Rate { get; set; } = 1f;
}
