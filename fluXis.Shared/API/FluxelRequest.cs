using fluXis.Shared.API.Packets;
using Newtonsoft.Json;

namespace fluXis.Shared.API;

public class FluxelRequest<T>
    where T : IPacket
{
    [JsonProperty("id")]
    public string ID { get; set; } = null!;

    [JsonProperty("data")]
    public T? Data { get; set; }

    public FluxelRequest(string id, T? data)
    {
        ID = id;
        Data = data;
    }
}
