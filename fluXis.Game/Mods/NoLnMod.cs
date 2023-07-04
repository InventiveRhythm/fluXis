using System;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class NoLnMod : IMod
{
    public string Name => "No LN";
    public string Acronym => "NLN";
    public string Description => "Removes all long notes and replaces them with single notes.";
    public IconUsage Icon => FontAwesome.Solid.ArrowsAltV;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => .8f;
    public bool Rankable => true;
    public string[] IncompatibleMods => Array.Empty<string>();
}
