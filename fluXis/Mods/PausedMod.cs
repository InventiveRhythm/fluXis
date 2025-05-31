using System;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class PausedMod : IMod
{
    public string Name => "Paused";
    public string Acronym => "PA";
    public string Description => "Paused the game mid-play.";
    public IconUsage Icon => FontAwesome6.Solid.Pause;
    public ModType Type => ModType.Special;
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => false;
    public bool SaveScore => true; // we dont want to submit paused scores but still show the on the local leaderboard
    public Type[] IncompatibleMods => Array.Empty<Type>();
}
