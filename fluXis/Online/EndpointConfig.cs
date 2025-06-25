using fluXis.Online.API.Models;

namespace fluXis.Online;

public class EndpointConfig
{
    public string APIUrl { get; private set; }
    public string AssetUrl { get; private set; }
    public string WebsiteRootUrl { get; private set; }
    public string WikiRootUrl { get; private set; }

    public EndpointConfig(string apiUrl)
    {
        APIUrl = apiUrl;
    }

    public EndpointConfig(string apiUrl, APIConfig config)
    {
        APIUrl = apiUrl;
        AssetUrl = config.AssetsUrl;
        WebsiteRootUrl = config.WebsiteUrl;
        WikiRootUrl = config.WikiUrl;
    }
}
