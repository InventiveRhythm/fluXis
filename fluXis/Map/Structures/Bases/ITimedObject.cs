using System.ComponentModel;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using JetBrains.Annotations;
using Newtonsoft.Json;

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
}
