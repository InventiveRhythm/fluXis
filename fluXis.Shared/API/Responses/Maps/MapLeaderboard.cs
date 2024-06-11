using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Scores;
using fluXis.Shared.Utils;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Responses.Maps;

public class MapLeaderboard
{
    [JsonProperty("map")]
    public APIMap Map { get; set; } = null!;

    [JsonProperty("scores")]
    public List<APIScore> Scores { get; set; } = new();

    public MapLeaderboard(APIMap map, IEnumerable<APIScore> scores)
    {
        Map = map;
        Scores = scores.ToList();
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public MapLeaderboard()
    {
    }
}
