using fluXis.Game.Online.Fluxel.Packets;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel;

public class FluxelRequest
{
    [JsonProperty("id")]
    public string ID;

    [JsonProperty("data")]
    public Packet Data;

    public FluxelRequest(string id, Packet data)
    {
        ID = id;
        Data = data;
    }
}
