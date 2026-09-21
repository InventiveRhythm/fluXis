using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures;
using fluXis.Mode.Keys.Objects;
using fluXis.Mods;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mode.Keys.Mods;

public class NoLnMod : IMod, IApplicableToHitObject
{
    public string Name => "No LN";
    public string Acronym => "NLN";
    public string Description => "Removes all long notes and replaces them with single notes.";
    public IconUsage Icon => Phosphor.Bold.ArrowsDownUp;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => .8f;
    public float RatingMultiplier => .6f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(HitObject hit)
    {
        if (hit is LongNote ln) ln.Duration = 0;
    }
}
