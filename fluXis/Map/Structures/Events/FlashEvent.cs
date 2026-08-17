using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace fluXis.Map.Structures.Events;

[Description("Overlays a solid color over the screen.")]
[Icon(FluXisIconType.Flash)]
public class FlashEvent : IMapEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("background")]
    public bool InBackground { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; } = Easing.None;

    [JsonProperty("start-color")]
    public Color4 StartColor { get; set; } = Color4.White;

    [JsonProperty("start-alpha")]
    public float StartOpacity { get; set; } = 1;

    [JsonProperty("end-color")]
    public Color4 EndColor { get; set; } = Color4.White;

    [JsonProperty("end-alpha")]
    public float EndOpacity { get; set; }

    public IEnumerable<Drawable> CreateObjectOverlay(EditorDrawableObject obj)
    {
        var color = new Box { Width = 12, RelativeSizeAxes = Axes.Y };
        yield return color;

        var opacity = new Box { Width = 12, RelativeSizeAxes = Axes.Y, X = 12 };
        yield return opacity;

        obj.DataUpdate += () =>
        {
            color.Colour = ColourInfo.GradientVertical(
                EndColor,
                StartColor
            );

            opacity.Colour = ColourInfo.GradientVertical(
                Colour4.White.Opacity(EndOpacity),
                Colour4.White.Opacity(StartOpacity)
            );
        };

        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        yield return flow;
    }
}
