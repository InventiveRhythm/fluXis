using Newtonsoft.Json;

namespace fluXis.Skinning.Json;

public class SkinJudgements
{
    [JsonProperty("flawless")]
    public string Flawless { get; set; } = "#00C3FF";

    [JsonProperty("perfect")]
    public string Perfect { get; set; } = "#22FFB5";

    [JsonProperty("great")]
    public string Great { get; set; } = "#4BFF3B";

    [JsonProperty("alright")]
    public string Alright { get; set; } = "#FFF12B";

    [JsonProperty("okay")]
    public string Okay { get; set; } = "#F7AD40";

    [JsonProperty("miss")]
    public string Miss { get; set; } = "#FF5555";
}
