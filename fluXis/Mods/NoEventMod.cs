using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class NoEventMod : IMod, IApplicableToEvents
{
    public string Name => "No Events";
    public string Acronym => "NEV";
    public string Description => "Removes all visual effects.";
    public IconUsage Icon => FontAwesome6.Solid.Diamond;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => 0.6f;
    public bool Rankable => false;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(MapEvents events)
    {
        events.FlashEvents.Clear();
        events.PulseEvents.Clear();
        events.PlayfieldMoveEvents.Clear();
        events.PlayfieldScaleEvents.Clear();
        events.PlayfieldRotateEvents.Clear();
        events.LayerFadeEvents.Clear();
        events.ShakeEvents.Clear();
        events.ShaderEvents.Clear();
        events.BeatPulseEvents.Clear();
    }
}
