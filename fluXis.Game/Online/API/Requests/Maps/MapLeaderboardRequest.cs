using System;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Screens.Select.Info.Scores;

namespace fluXis.Game.Online.API.Requests.Maps;

public class MapLeaderboardRequest : APIRequest<APIScores>
{
    protected override string Path => type switch
    {
        ScoreListType.Global => $"/map/{id}/scores",
        ScoreListType.Country => $"/map/{id}/scores/country",
        ScoreListType.Friends => $"/map/{id}/scores/friends",
        ScoreListType.Club => $"/map/{id}/scores/club",
        _ => throw new ArgumentOutOfRangeException()
    };

    private ScoreListType type { get; }
    private int id { get; }

    public MapLeaderboardRequest(ScoreListType type, int id)
    {
        this.type = type;
        this.id = id;
    }
}
