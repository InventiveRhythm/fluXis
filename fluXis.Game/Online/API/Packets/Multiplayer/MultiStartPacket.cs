using fluXis.Game.Online.Fluxel;

namespace fluXis.Game.Online.API.Packets.Multiplayer;

public class MultiStartPacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_START;
}
