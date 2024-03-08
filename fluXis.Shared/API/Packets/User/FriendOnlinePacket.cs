using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.User;

public class FriendOnlinePacket : IPacket
{
    public string ID => online ? PacketIDs.FRIEND_ONLINE : PacketIDs.FRIEND_OFFLINE;

    [JsonProperty("user")]
    public APIUserShort? User { get; set; }

    private bool online;

    public static FriendOnlinePacket CreateS2C(APIUserShort user, bool online)
        => new() { User = user, online = online };
}
