using fluXis.Online.API.Models.Clubs;
using fluXis.Online.API.Models.Scores;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Maps;

public class APIMapClaim
{
    [JsonProperty("club")]
    public APIClub Club { get; init; }

    [JsonProperty("score")]
    public APIClubScore Score { get; init; }
}
