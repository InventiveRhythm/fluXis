using Newtonsoft.Json;
using SixLabors.ImageSharp;

namespace fluXis.Game.Skinning.Json;

public class SkinJudgements : IDeepCloneable<SkinJudgements>
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

    public SkinJudgements DeepClone() => new()
    {
        Flawless = Flawless,
        Perfect = Perfect,
        Great = Great,
        Alright = Alright,
        Okay = Okay,
        Miss = Miss
    };
}
