namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

/// <summary>
/// Player has finished the map and is waiting for others to finish.
/// </summary>
public class MultiplayerCompletePacket : Packet
{
    public override string ID => "multi/complete";
}
