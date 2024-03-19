using fluXis.Shared.API.Packets;
using Newtonsoft.Json;

namespace fluXis.Shared.API;

public class FluxelRequest<T>
    where T : IPacket
{
    [JsonProperty("id")]
    public string ID { get; set; }

    [JsonProperty("token")]
    public Guid Token { get; set; }

    [JsonProperty("data")]
    public T? Data { get; set; }

    public FluxelRequest(string id, T? data)
    {
        ID = id;
        Token = Guid.NewGuid();
        Data = data;
    }
}
