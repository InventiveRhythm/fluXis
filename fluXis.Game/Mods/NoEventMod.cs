using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

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
        events.PlayfieldFadeEvents.Clear();
        events.PlayfieldRotateEvents.Clear();
        events.HitObjectFadeEvents.Clear();
        events.HitObjectEaseEvents.Clear();
        events.ShakeEvents.Clear();
        events.ShaderEvents.Clear();
        events.BeatPulseEvents.Clear();
        events.ScrollMultiplyEvents.Clear();
        events.TimeOffsetEvents.Clear();
    }
}
