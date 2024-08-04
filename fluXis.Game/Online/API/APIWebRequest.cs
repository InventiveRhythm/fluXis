using fluXis.Shared.Utils;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.API;

public class APIWebRequest<T> : JsonWebRequest<APIResponse<T>>
{
    protected override string UserAgent => "fluXis";

    public new APIResponse<T> ResponseObject { get; private set; }

    private bool logOutput { get; }

    public APIWebRequest(string url, bool logOutput = false)
        : base(url)
    {
        AllowInsecureRequests = true;
        this.logOutput = logOutput;
    }

    protected override void ProcessResponse()
    {
        var response = GetResponseString();

        if (logOutput)
            Logger.Log(response, LoggingTarget.Network);

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
