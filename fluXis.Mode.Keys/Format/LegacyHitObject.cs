using System;
using fluXis.Map.Structures.Bases;
using fluXis.Scoring.Structs;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Mode.Keys.Format;

[Obsolete]
public class LegacyHitObject : IHasDuration
{
    #region Stored

    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    /// <summary>
    /// the visual position of the note. (only applies to tick notes)
    /// </summary>
    [JsonProperty("visual-lane", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float VisualLane { get; set; }

    [JsonProperty("holdtime", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public double HoldTime { get; set; }

    [JsonProperty("hitsound")]
    public string HitSound { get; set; } = string.Empty;

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; } = string.Empty;

    [JsonProperty("hidden")]
    public bool Hidden { get; set; }

    [JsonProperty("type")]
    public LegacyHitObjectType Type { get; set; }

    #endregion

    [JsonIgnore]
    public bool LongNote => HoldTime > 0 && Type == LegacyHitObjectType.Normal;

    [JsonIgnore]
    public bool Landmine => Type == LegacyHitObjectType.Landmine;

    [JsonIgnore]
    public double EndTime
    {
        get
        {
            if (HoldTime <= 0)
                return Time;

            return Time + HoldTime;
        }
        set => HoldTime = value - Time;
    }

    [JsonIgnore]
    public HitResult? Result { get; set; }

    [JsonIgnore]
    public HitResult? HoldEndResult { get; set; }

    /// <summary>
    /// The ease type the start of this note has.
    /// </summary>
    [JsonIgnore]
    public Easing StartEasing { get; set; } = Easing.None;

    /// <summary>
    /// The ease type the end of this note has.
    /// </summary>
    [JsonIgnore]
    public Easing EndEasing { get; set; } = Easing.None;

    double IHasDuration.Duration { get => HoldTime; set => HoldTime = value; }
}

public enum LegacyHitObjectType
{
    Normal,
    Tick,
    Landmine
}
