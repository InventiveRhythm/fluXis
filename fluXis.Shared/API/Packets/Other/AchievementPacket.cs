using fluXis.Shared.Components.Other;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Other;

public class AchievementPacket : IPacket
{
    public string ID => PacketIDs.ACHIEVEMENT;

    [JsonProperty("achievement")]
    public Achievement Achievement { get; set; } = null!;

    public static AchievementPacket Create(Achievement achievement)
        => new() { Achievement = achievement };
}
