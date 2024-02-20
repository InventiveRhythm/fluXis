using Newtonsoft.Json;

namespace fluXis.Game.Map.Events.Shader;

public class ShaderStrengthParams
{
    [JsonProperty("strength")]
    public float Strength { get; set; }
}
