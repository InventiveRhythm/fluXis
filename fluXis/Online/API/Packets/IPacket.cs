using Newtonsoft.Json;

namespace fluXis.Online.API.Packets;

public interface IPacket
{
    [JsonIgnore]
    string ID { get; }
}
