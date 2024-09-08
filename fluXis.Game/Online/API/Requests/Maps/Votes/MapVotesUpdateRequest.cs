using System.Net.Http;
using fluXis.Shared.API.Payloads.Maps;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Maps.Votes;

public class MapVotesUpdateRequest : APIRequest<APIMapVotes>
{
    protected override string Path => $"/map/{id}/votes";
    protected override HttpMethod Method => HttpMethod.Post;

    private long id { get; }
    private int value { get; }

    public MapVotesUpdateRequest(long id, int value)
    {
        this.id = id;
        this.value = value;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new MapVotePayload { YourVote = value }.Serialize());
        return req;
    }
}
