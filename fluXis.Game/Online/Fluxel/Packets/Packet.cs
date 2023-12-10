using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets;

public abstract class Packet
{
    [JsonIgnore]
    public abstract string ID { get; }
}
