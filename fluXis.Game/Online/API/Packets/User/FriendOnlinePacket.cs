using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Packets.User;

#nullable enable

public class FriendOnlinePacket : IPacket
{
    public string ID => online ? PacketIDs.FRIEND_ONLINE : PacketIDs.FRIEND_OFFLINE;

    [JsonProperty("user")]
    public APIUser? User { get; set; }

    private bool online;

    public static FriendOnlinePacket CreateS2C(APIUser user, bool online)
        => new() { User = user, online = online };
}
