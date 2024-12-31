using fluXis.Online.Fluxel;

namespace fluXis.Online.API.Packets.Other;

public class MaintenancePacket : IPacket
{
    public string ID => PacketIDs.MAINTENANCE;

    /// <summary>
    /// Unix timestamp of the time the server will go down.
    /// </summary>
    public long Time { get; init; }

    public static MaintenancePacket CreateS2C(long time)
        => new() { Time = time };
}
