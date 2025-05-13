using System.Net.Http;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.MapSets;

public class MapSetSubmitQueueRequest : APIRequest<dynamic>
{
    protected override string Path => $"/mapset/{id}/submit";
    protected override HttpMethod Method => HttpMethod.Post;

    private long id { get; }

    public MapSetSubmitQueueRequest(long id)
    {
        this.id = id;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw("{}");
        return req;
    }
}
