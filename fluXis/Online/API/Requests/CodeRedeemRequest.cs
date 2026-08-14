using System.Net.Http;
using fluXis.Online.API.Payloads;
using Midori.Utils;
using Newtonsoft.Json.Linq;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests;

public class CodeRedeemRequest : APIRequest<JObject>
{
    protected override string Path => "/codes/redeem";
    protected override HttpMethod Method => HttpMethod.Post;

    private CodeRedeemPayload payload { get; }

    public CodeRedeemRequest(CodeRedeemPayload payload)
    {
        this.payload = payload;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        var json = payload.Serialize();
        req.AddRaw(json);
        return req;
    }
}
