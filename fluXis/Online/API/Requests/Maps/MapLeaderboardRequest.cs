using fluXis.Online.API.Responses.Maps;
using fluXis.Screens.Select.Info.Tabs.Scores;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Maps;

public class MapLeaderboardRequest : APIRequest<MapLeaderboard>
{
    protected override string Path => $"/map/{id}/scores";

    private ScoreListType type { get; }
    private long id { get; }
    private long playerIndex { get; }

    public MapLeaderboardRequest(ScoreListType type, long id, int playerIndex = 0)
    {
        this.type = type;
        this.id = id;
        this.playerIndex = playerIndex;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddParameter("type", type.ToString().ToLower(), RequestParameterType.Query);
        req.AddParameter("playerIndex", playerIndex.ToString(), RequestParameterType.Query);
        return req;
    }
}
