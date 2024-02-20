using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;
using Newtonsoft.Json.Linq;

namespace fluXis.Game.Map.Events;

public class ShaderEvent : TimedObject
{
    public string ShaderName { get; set; } = string.Empty;
    public JObject ShaderParams { get; set; } = new();

    public T ParamsAs<T>() => ShaderParams.ToObject<T>();

    public override string ToString() => $"Shader({Time.ToStringInvariant()},{ShaderName},{ShaderParams.Serialize()})";
}
