using System.Net.Http;
using fluXis.Shared.API.Payloads.Users;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Users;

public class UserProfileUpdateRequest : APIRequest<APIUser>
{
    protected override string Path => $"/user/{uid}";
    protected override HttpMethod Method => HttpMethod.Patch;

    private long uid { get; }
    private UserProfileUpdatePayload payload { get; }

    public UserProfileUpdateRequest(long uid, UserProfileUpdatePayload payload)
    {
        this.uid = uid;
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
