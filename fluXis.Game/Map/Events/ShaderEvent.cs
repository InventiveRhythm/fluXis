using fluXis.Game.Map.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Game.Map.Events;

public class ShaderEvent : TimedObject
{
    [JsonProperty("shader")]
    public string ShaderName { get; set; } = string.Empty;

    [JsonProperty("params")]
    public JObject ShaderParams { get; set; } = new();

    public T ParamsAs<T>() => ShaderParams.ToObject<T>();
}
