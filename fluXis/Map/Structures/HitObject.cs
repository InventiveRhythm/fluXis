using System.ComponentModel;
using fluXis.Map.Structures.Bases;
using fluXis.Scoring.Structs;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Gameplay.Ruleset;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures;

public class HitObject : ITimedObject
{
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
    public string HitSound { get; set; }

    [DefaultValue("")]
    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("hidden")]
    public bool Hidden { get; set; }

    /// <summary>
    /// 0 = Normal / Long
    /// 1 = Tick
    /// 2 = Landmine
    /// </summary>
    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonIgnore]
    public bool LongNote => HoldTime > 0 && Type == 0;

    [JsonIgnore]
    public bool Landmine => Type == 2;

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
    /// The next HitObject in the same lane.
    /// </summary>
    [CanBeNull]
    [JsonIgnore]
    public HitObject NextObject { get; set; }

    /// <summary>
    /// The scroll group for this object.
    /// </summary>
    [CanBeNull]
    [JsonIgnore]
    public ScrollGroup ScrollGroup { get; set; }

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

    [CanBeNull]
    [JsonIgnore]
    public EditorHitObject EditorDrawable { get; set; }
}
