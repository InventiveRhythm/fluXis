using fluXis.Game.Online.Fluxel.Packets;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel;

public class FluxelRequest
{
    [JsonProperty("id")]
    public int ID;

    [JsonProperty("data")]
    public Packet Data;

    public FluxelRequest(int id, Packet data)
    {
        ID = id;
        Data = data;
    }
}