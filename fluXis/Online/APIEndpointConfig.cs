using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online;

public class APIEndpointConfig
{
    [JsonProperty("api")]
    public string APIUrl { get; private set; }

    [JsonProperty("assets")]
    public string AssetUrl { get; private set; }

    [JsonProperty("website")]
    public string WebsiteRootUrl { get; private set; }

    [JsonProperty("wiki")]
    public string WikiRootUrl { get; private set; }

    public APIEndpointConfig AddDefaults()
    {
        if (string.IsNullOrWhiteSpace(APIUrl) || APIUrl.EqualsLower("https://api.fluxis.flux.moe"))
            APIUrl = "https://fluxis.flux.moe/api";
        if (string.IsNullOrWhiteSpace(AssetUrl))
            AssetUrl = "https://assets.flux.moe";
        if (string.IsNullOrWhiteSpace(WebsiteRootUrl))
            WebsiteRootUrl = "https://fluxis.flux.moe";
        if (string.IsNullOrWhiteSpace(WikiRootUrl))
            WikiRootUrl = "https://fluxis.flux.moe/wiki";

        return this;
    }

    public APIEndpointConfig AddLocalDefaults()
    {
        APIUrl = "http://localhost:2434";
        AssetUrl = "http://localhost:2434/assets";
        WebsiteRootUrl = "http://localhost:2432";
        WikiRootUrl = "http://localhost:2432/wiki";
        return this;
    }
}
