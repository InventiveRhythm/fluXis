using System.Net.Http;
using fluXis.Shared.API.Parameters.Users;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.API.Requests.Users;

public class UserProfileUpdateRequest : APIRequest<APIUser>
{
    protected override string Path => $"/user/{uid}/profile";
    protected override HttpMethod Method => HttpMethod.Patch;

    private long uid { get; }
    private UserProfileUpdateParameters parameters { get; }

    public UserProfileUpdateRequest(long uid, UserProfileUpdateParameters parameters)
    {
        this.uid = uid;
        this.parameters = parameters;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        var json = parameters.Serialize();
        Logger.Log($"{json}", LoggingTarget.Network, LogLevel.Debug);
        req.AddRaw(json);
        return req;
    }
}
