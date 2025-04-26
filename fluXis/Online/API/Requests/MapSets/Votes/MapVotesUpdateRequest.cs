using System.Net.Http;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Payloads.Maps;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.MapSets.Votes;

public class MapVotesUpdateRequest : APIRequest<APIMapVotes>
{
    protected override string Path => $"/mapset/{id}/votes";
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
