using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets;

public abstract class Packet
{
    [JsonIgnore]
    public abstract int ID { get; }
}
