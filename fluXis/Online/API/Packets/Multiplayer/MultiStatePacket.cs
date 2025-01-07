using fluXis.Online.API.Models.Multi;
using fluXis.Online.Fluxel;

namespace fluXis.Online.API.Packets.Multiplayer;

public class MultiStatePacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_STATE;

    public MultiplayerUserState State { get; set; }

    #region Server2Client

    public long UserID { get; set; }

    #endregion

    public static MultiStatePacket CreateC2S(MultiplayerUserState state)
        => new() { State = state };

    public static MultiStatePacket CreateS2C(long userId, MultiplayerUserState state)
        => new() { UserID = userId, State = state };
}
