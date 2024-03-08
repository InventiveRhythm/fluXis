using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets;

public interface IPacket
{
    [JsonIgnore]
    string ID { get; }
}
