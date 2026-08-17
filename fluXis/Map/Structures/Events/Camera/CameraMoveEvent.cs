using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures.Events.Camera;

[Description("Moves the camera.")]
public class CameraMoveEvent : ICameraEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("x")]
    public float X { get; set; }

    [JsonProperty("y")]
    public float Y { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    public void Apply(Drawable draw)
    {
        using (draw.BeginAbsoluteSequence(Time))
        {
            draw.MoveToX(X, Math.Max(Duration, 0), Easing);
            draw.MoveToY(Y, Math.Max(Duration, 0), Easing);
        }
    }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{(int)X}x {(int)Y}y"));
        yield return flow;
    }
}
