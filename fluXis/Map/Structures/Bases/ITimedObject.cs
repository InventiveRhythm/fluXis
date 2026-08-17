using System;
using System.Collections.Generic;
using System.ComponentModel;
using fluXis.Graphics.Sprites.Text;
using fluXis.Screens.Edit;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Utils.Inspect;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Map.Structures.Bases;

public interface ITimedObject
{
    [JsonProperty("time")]
    double Time { get; set; }

    [JsonProperty("lane")]
    int Lane { get; set; }

    [DefaultValue("")]
    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    string Group { get; set; }

    [CanBeNull]
    PlacementBlueprint CreateEditorBlueprint() => null;

    static EditorVariableTime CreateVariableTime(ObjectProperty _, object obj, object ctx)
        => new((EditorMap)ctx, (ITimedObject)obj);

    IEnumerable<Drawable> CreateObjectOverlay(EditorDrawableObject obj)
        => CreateDefaultOverlay(obj);

    static IEnumerable<Drawable> CreateDefaultOverlay(EditorDrawableObject obj)
    {
        var flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(-4),
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Masking = true,
            Child = obj.GroupText = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = obj.TextColor,
                WebFontSize = 12
            }
        };

        obj.OnUpdate += _ => flow.Height = 36 * obj.Zoom;
        yield return flow;
    }

    protected static Drawable CreateSmallText(EditorDrawableObject obj, Func<string> update)
    {
        var text = new FluXisSpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Colour = obj.TextColor,
            WebFontSize = 10
        };

        obj.DataUpdate += () => text.Text = update();
        return text;
    }
}
