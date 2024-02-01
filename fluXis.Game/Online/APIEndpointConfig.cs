using fluXis.Game.Utils;
using Newtonsoft.Json;

namespace fluXis.Game.Online;

public class APIEndpointConfig
{
    [JsonProperty("api")]
    public string APIUrl { get; private set; } = "https://api.fluxis.flux.moe";

    [JsonProperty("assets")]
    public string AssetUrl { get; private set; } = "https://assets.flux.moe";

    [JsonProperty("websocket")]
    public string WebsocketUrl { get; private set; } = "wss://fluxel.flux.moe";

    [JsonProperty("website")]
    public string WebsiteRootUrl { get; private set; } = "https://fluxis.flux.moe";

    public override string ToString() => this.Serialize(true);
}
