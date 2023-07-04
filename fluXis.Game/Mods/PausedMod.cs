using System;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class PausedMod : IMod
{
    public string Name => "Paused";
    public string Acronym => "PA";
    public string Description => "Paused the game mid-play.";
    public IconUsage Icon => FontAwesome.Solid.Pause;
    public ModType Type => ModType.Special;
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => true;
    public string[] IncompatibleMods => Array.Empty<string>();
}
