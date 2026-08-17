using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures;

public class HitSoundFade : ITimedObject
{
    /// <summary>
    /// The time at which the volume change should start.
    /// </summary>
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    /// <summary>
    /// The sound to change the volume of.
    /// </summary>
    [JsonProperty("sound")]
    public string HitSound { get; set; }

    /// <summary>
    /// The volume to fade to.
    /// </summary>
    [JsonProperty("volume")]
    public double Volume { get; set; }

    /// <summary>
    /// The duration of the fade.
    /// </summary>
    [JsonProperty("duration")]
    public float Duration { get; set; }

    /// <summary>
    /// The easing function to use for the fade.
    /// </summary>
    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    [JsonIgnore]
    string ITimedObject.Group { get; set; }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => HitSound));
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{(int)(Volume * 100)}%"));
        yield return flow;
    }
}
