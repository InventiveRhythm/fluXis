using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Screens.Select.Info.Scores;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Maps;

public class MapLeaderboardRequest : APIRequest<APIScores>
{
    protected override string Path => $"/map/{id}/scores";

    private ScoreListType type { get; }
    private long id { get; }

    public MapLeaderboardRequest(ScoreListType type, long id)
    {
        this.type = type;
        this.id = id;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddParameter("type", type.ToString().ToLower(), RequestParameterType.Query);
        return req;
    }
}
