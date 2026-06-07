using System.ComponentModel;
using Newtonsoft.Json;

namespace fluXis.Map.Structures.Bases;

public interface ITimedObject
{
    [JsonProperty("time")]
    double Time { get; set; }

    [DefaultValue("")]
    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    string Group { get; set; }
}
