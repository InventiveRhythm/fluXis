using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets;

public class Packet
{
    [JsonIgnore]
    public int ID { get; }

    public Packet(int id)
    {
        ID = id;
    }
}