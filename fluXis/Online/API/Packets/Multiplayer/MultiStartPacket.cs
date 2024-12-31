using fluXis.Online.Fluxel;

namespace fluXis.Online.API.Packets.Multiplayer;

public class MultiStartPacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_START;
}
