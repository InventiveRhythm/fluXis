﻿using System;
using System.Collections.Generic;
using fluXis.Online.API.Models.Users;
using Newtonsoft.Json;
using osu.Framework.Localisation;

namespace fluXis.Online.API.Models.Maps;

public class APIMapSet
{
    [JsonProperty("id")]
    public long ID { get; set; }

    [JsonProperty("creator")]
    public APIUser Creator { get; set; } = APIUser.CreateUnknown(0);

    [JsonProperty("maps")]
    public List<APIMap> Maps { get; set; } = new();

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("title-rm")]
    public string TitleRomanized { get; set; } = string.Empty;

    [JsonProperty("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonProperty("artist-rm")]
    public string ArtistRomanized { get; set; } = string.Empty;

    [JsonProperty("source")]
    public string Source { get; set; } = "";

    [JsonProperty("tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    [JsonProperty("flags")]
    public MapSetFlag Flags { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("submitted")]
    public long DateSubmitted { get; set; }

    [JsonProperty("updated")]
    public long LastUpdated { get; set; }

    #region Localization

    [JsonIgnore]
    public RomanisableString LocalizedTitle => new(Title, TitleRomanized);

    [JsonIgnore]
    public RomanisableString LocalizedArtist => new(Artist, ArtistRomanized);

    #endregion
}
