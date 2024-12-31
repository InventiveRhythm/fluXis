using fluXis.Online.Fluxel;

namespace fluXis.Online.API.Packets.Account;

public class LogoutPacket : IPacket
{
    public string ID => PacketIDs.LOGOUT;
}
