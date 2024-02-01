using fluXis.Game.Online.API;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel;

public class FluxelResponse<T> : APIResponse<T>
{
    [JsonProperty("id")]
    public string Type;

    public FluxelResponse(int status, string message, T data, string type)
        : base(status, message, data)
    {
        Type = type;
    }
}
