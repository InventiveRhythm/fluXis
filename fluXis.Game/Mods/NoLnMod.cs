using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Structures;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class NoLnMod : IMod, IApplicableToHitObject
{
    public string Name => "No LN";
    public string Acronym => "NLN";
    public string Description => "Removes all long notes and replaces them with single notes.";
    public IconUsage Icon => FontAwesome6.Solid.UpDown;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => .8f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(HitObject hit) => hit.HoldTime = 0;
}
