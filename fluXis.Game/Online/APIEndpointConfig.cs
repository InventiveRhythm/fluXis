using Newtonsoft.Json;

namespace fluXis.Game.Online;

public class APIEndpointConfig
{
    [JsonProperty("api")]
    public string APIUrl { get; private set; }

    [JsonProperty("assets")]
    public string AssetUrl { get; private set; }

    [JsonProperty("websocket")]
    public string WebsocketUrl { get; private set; }

    [JsonProperty("website")]
    public string WebsiteRootUrl { get; private set; }

    [JsonProperty("wiki")]
    public string WikiRootUrl { get; private set; }

    public APIEndpointConfig AddDefaults()
    {
        if (string.IsNullOrWhiteSpace(APIUrl))
            APIUrl = "https://fluxis.flux.moe/api";
        if (string.IsNullOrWhiteSpace(AssetUrl))
            AssetUrl = "https://assets.flux.moe";
        if (string.IsNullOrWhiteSpace(WebsocketUrl))
            WebsocketUrl = "wss://fluxis.flux.moe/socket";
        if (string.IsNullOrWhiteSpace(WebsiteRootUrl))
            WebsiteRootUrl = "https://fluxis.flux.moe";
        if (string.IsNullOrWhiteSpace(WikiRootUrl))
            WikiRootUrl = "https://fluxis.flux.moe/wiki";

        return this;
    }
}
