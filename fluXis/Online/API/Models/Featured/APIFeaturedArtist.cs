using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Featured;

#nullable enable

public class APIFeaturedArtist
{
    [JsonProperty("id")]
    public string ID { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("youtube")]
    public string? YouTube { get; set; }

    [JsonProperty("spotify")]
    public string? Spotify { get; set; }

    [JsonProperty("soundcloud")]
    public string? SoundCloud { get; set; }

    [JsonProperty("twitter")]
    public string? Twitter { get; set; }

    [JsonProperty("fluxis")]
    public string? FluXis { get; set; }

    [JsonProperty("albums")]
    public List<APIFeaturedAlbum> Albums { get; set; } = new();
}
