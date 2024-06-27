using fluXis.Shared.Utils;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.API;

public class APIWebRequest<T> : JsonWebRequest<APIResponse<T>>
{
    protected override string UserAgent => "fluXis";

    public new APIResponse<T> ResponseObject { get; private set; }

    public APIWebRequest(string url)
        : base(url)
    {
        AllowInsecureRequests = true;
    }

    protected override void ProcessResponse()
    {
        var response = GetResponseString();
        Logger.Log(response, LoggingTarget.Network, LogLevel.Debug);

        if (response != null)
            ResponseObject = response.Deserialize<APIResponse<T>>();
    }
}

public class APIWebRequest : WebRequest
{
    protected override string UserAgent => "fluXis";

    public APIWebRequest(string url = null)
        : base(url)
    {
        AllowInsecureRequests = true;
    }
}
