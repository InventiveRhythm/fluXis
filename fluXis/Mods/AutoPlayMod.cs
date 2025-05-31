using System;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class AutoPlayMod : IMod
{
    public string Name => "AutoPlay";
    public string Acronym => "AP";
    public string Description => "Watch a perfect replay of the map.";
    public IconUsage Icon => FontAwesome6.Solid.Plane;
    public ModType Type => ModType.Automation;
    public float ScoreMultiplier => 1.0f;
    public bool Rankable => false;
    public Type[] IncompatibleMods => new[] { typeof(NoFailMod), typeof(FragileMod), typeof(FlawlessMod) };
}
