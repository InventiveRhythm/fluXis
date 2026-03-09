using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using fluXis.Online.API.Models.Maps;
using Newtonsoft.Json;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Maps;

public class MapHashesRequest : APIRequest<APIMapHashes>
{
    protected override string Path => $"/maps/hashes";
    protected override HttpMethod Method => HttpMethod.Post;

    private IEnumerable<long> ids { get; }

    public MapHashesRequest(IEnumerable<long> ids)
    {
        this.ids = ids;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        var json = JsonConvert.SerializeObject(ids);
        req.AddRaw(Encoding.UTF8.GetBytes(json));
        return req;
    }
}