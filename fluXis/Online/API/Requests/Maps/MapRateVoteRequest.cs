using System.Net.Http;
using fluXis.Online.API.Payloads.Maps;
using fluXis.Utils;
using WebRequest = osu.Framework.IO.Network.WebRequest;

namespace fluXis.Online.API.Requests.Maps;

public class MapRateVoteRequest : APIRequest<string>
{
    protected override string Path => $"/map/{id}/rate";
    protected override HttpMethod Method => HttpMethod.Post;

    private long id { get; }
    private MapRateVotePayload payload { get; }

    public MapRateVoteRequest(long id, MapRateVotePayload payload)
    {
        this.id = id;
        this.payload = payload;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(payload.Serialize());
        return req;
    }
}
