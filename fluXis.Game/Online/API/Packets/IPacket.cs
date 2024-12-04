using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Packets;

public interface IPacket
{
    [JsonIgnore]
    string ID { get; }
}
