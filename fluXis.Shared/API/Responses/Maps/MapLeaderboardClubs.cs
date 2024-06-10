using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Scores;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Responses.Maps;

public class MapLeaderboardClubs
{
    [JsonProperty("map")]
    public APIMap Map { get; set; } = null!;

    [JsonProperty("scores")]
    public List<APIClubScore> Scores { get; set; } = new();

    public MapLeaderboardClubs(APIMap map, IEnumerable<APIClubScore> scores)
    {
        Map = map;
        Scores = scores.ToList();
    }

    [JsonConstructor]
    [Obsolete("This constructor is for json parsing only.", true)]
    public MapLeaderboardClubs()
    {
    }
}
