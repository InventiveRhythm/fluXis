using System;
using System.Linq;
using System.Net.Http;
using fluXis.Game.Online.Fluxel;
using JetBrains.Annotations;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.API;

public abstract class APIRequest<T> : APIRequest
    where T : class
{
    public new event Action<APIResponse<T>> Success;

    public APIResponse<T> Response { get; protected set; }

    public override bool IsSuccessful => Response?.Success ?? false;

    protected APIRequest()
    {
        base.Success += () => Success?.Invoke(Response);
    }

    protected override void PostProcess()
    {
        if (Request is not APIWebRequest<T> req)
            throw new InvalidOperationException("Request was not of expected type.");

        Response = req.ResponseObject;

        if (!Response.Success)
            Fail(new APIException(Response.Message));
    }

    // used for testing
    public void TriggerSuccess(APIResponse<T> res)
    {
        Response = res;
        TriggerSuccess();
    }

    protected override WebRequest CreateWebRequest(string url) => new APIWebRequest<T>(url);
}

public abstract class APIRequest
{
    protected abstract string Path { get; }
    protected virtual HttpMethod Method => HttpMethod.Get;
    protected virtual string RootUrl => APIClient.Endpoint.APIUrl;

    protected FluxelClient APIClient { get; private set; }
    protected WebRequest Request { get; private set; }

    public event Action Success;
    public event Action<Exception> Failure;
    public event Action<long, long> Progress;

    public virtual bool IsSuccessful => Request.Completed;

    [CanBeNull]
    public Exception FailReason { get; private set; }

    public const string UNKNOWN_ERROR = "An unknown error occured.";

    private bool failed;

    /// <summary>
    /// error message when multifactor is required
    /// </summary>
    public const string MULTIFACTOR_REQUIRED = "mfa-required";

    public void Perform(IAPIClient client)
    {
        if (client is not FluxelClient fluxel)
            throw new InvalidOperationException($"APIRequest must be performed with a {nameof(FluxelClient)}.");

        APIClient = fluxel;

        Request = CreateWebRequest($"{RootUrl}{Path}");
        Request.Method = Method;
        Request.AllowRetryOnTimeout = false;
        Request.UploadProgress += (current, total) => Progress?.Invoke(current, total);
        Request.Failed += Fail;

        if (!string.IsNullOrEmpty(APIClient.AccessToken))
            Request.AddHeader("Authorization", APIClient.AccessToken);

        if (!string.IsNullOrEmpty(APIClient.MultifactorToken))
            Request.AddHeader("X-Multifactor-Token", APIClient.MultifactorToken);

        try
        {
            Logger.Log($"Performing API request {GetType().Name.Split('.').Last()}...", LoggingTarget.Network);
            Request.Perform();
        }
        catch { }

        if (failed) return;

        PostProcess();
        TriggerSuccess();
    }

    protected virtual void PostProcess()
    {
    }

    public void TriggerSuccess()
    {
        // failure might have been triggered during post-process.
        if (failed)
            return;

        if (APIClient == null)
            Success?.Invoke();
        else
            APIClient.Schedule(() => Success?.Invoke());
    }

    public void TriggerFailure(Exception e)
    {
        failed = true;

        if (APIClient == null)
            Failure?.Invoke(e);
        else
            APIClient.Schedule(() => Failure?.Invoke(e));
    }

    public void Fail(Exception e)
    {
        FailReason = e;
        Request?.Abort();
        Logger.Error(e, $"API request {GetType().Name.Split('.').Last()} failed!");
        TriggerFailure(e);
    }

    protected virtual WebRequest CreateWebRequest(string url) => new APIWebRequest(url);
}
