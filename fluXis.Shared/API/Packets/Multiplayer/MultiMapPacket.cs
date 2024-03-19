using fluXis.Shared.Components.Maps;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Multiplayer;

public class MultiMapPacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_MAP;

    [JsonProperty("id")]
    public long MapID { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;

    [JsonProperty("map")]
    public IAPIMapShort Map { get; set; } = null!;

    public static MultiMapPacket CreateC2S(long map, string hash)
        => new() { MapID = map, Hash = hash };

    public static MultiMapPacket CreateS2C(IAPIMapShort map)
        => new() { Map = map };
}
