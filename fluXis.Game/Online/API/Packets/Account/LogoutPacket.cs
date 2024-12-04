using fluXis.Game.Online.Fluxel;

namespace fluXis.Game.Online.API.Packets.Account;

public class LogoutPacket : IPacket
{
    public string ID => PacketIDs.LOGOUT;
}
