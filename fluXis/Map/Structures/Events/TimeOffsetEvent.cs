using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Ruleset.HitObjects;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Events;

public class TimeOffsetEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToHitManager
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("use-start")]
    public bool UseStartValue { get; set; }

    [JsonProperty("start-offset")]
    public double StartOffset { get; set; }

    [JsonProperty("offset")]
    public double TargetOffset { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    public void Apply(HitObjectManager manager)
    {
        using (manager.BeginAbsoluteSequence(Time))
        {
            if (UseStartValue)
                manager.TransformTo(nameof(manager.VisualTimeOffset), StartOffset);

            manager.TransformTo(nameof(manager.VisualTimeOffset), TargetOffset, Duration, Easing);
        }
    }
}
