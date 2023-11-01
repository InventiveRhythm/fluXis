using System;
using System.Net.Http;
using System.Threading.Tasks;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.API;

public class APIRequest<T> where T : class
{
    protected virtual string Path => string.Empty;
    protected virtual HttpMethod Method => HttpMethod.Get;

    public event Action<APIResponse<T>> Success;
    public event Action<Exception> Failure;
    public event Action<long, long> Progress;

    public APIResponse<T> Response { get; protected set; }

    private Fluxel.Fluxel fluxel;
    private APIEndpointConfig config => fluxel.Endpoint;

    public virtual void Perform(Fluxel.Fluxel fluxel)
    {
        this.fluxel = fluxel;

        var request = new JsonWebRequest<APIResponse<T>>($"{config.APIUrl}{Path}");
        request.Method = Method;
        request.AllowInsecureRequests = true;
        request.UploadProgress += (current, total) => Progress?.Invoke(current, total);

        if (!string.IsNullOrEmpty(fluxel.Token))
            request.AddHeader("Authorization", fluxel.Token);

        try
        {
            CreatePostData(request);
            request.Perform();
            Logger.Log($"API request performed: {request.GetResponseString()}", LoggingTarget.Network);
            TriggerSuccess(request.ResponseObject);
        }
        catch (Exception e)
        {
            Logger.Error(e, $"API request failed: {e.Message}", LoggingTarget.Network);
            Response = new APIResponse<T>(400, e.Message, null);
            Failure?.Invoke(e);
        }
    }

    protected void TriggerSuccess(APIResponse<T> res)
    {
        Response = res;

        if (res != null)
            fluxel.Schedule(() => Success?.Invoke(Response));
    }

    public Task PerformAsync(Fluxel.Fluxel fluxel)
    {
        var task = new Task(() => Perform(fluxel));
        task.Start();
        return task;
    }

    protected virtual void CreatePostData(JsonWebRequest<APIResponse<T>> request) { }
}
