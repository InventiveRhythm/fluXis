using System;
using fluXis.Graphics.Sprites.Icons;
using Midori.Utils.Extensions;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class RateMod : IMod
{
    public string Name => "Rate";
    public string Description => "Change the rate of the map";
    public IconUsage Icon => Phosphor.Bold.Clock;
    public ModType Type => ModType.Rate;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public string Acronym => $"{Math.Round(Rate, 2).ToStringInvariant()}x";
    public float ScoreMultiplier => 1f + (Rate - 1f) * 0.4f;
    public float Rate { get; set; } = 1f;

    public float RatingMultiplier
    {
        get
        {
            // https://www.geogebra.org/calculator/fjxrbmdq
            if (Rate < 1)
                return (float)Math.Pow(Rate, 3);

            return 1.5f * Rate - .5f;
        }
    }
}
