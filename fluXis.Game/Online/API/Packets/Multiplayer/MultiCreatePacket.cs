using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.Fluxel;

namespace fluXis.Game.Online.API.Packets.Multiplayer;

public class MultiCreatePacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_CREATE;

    #region Client2Server

    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public long MapID { get; set; }
    public string MapHash { get; set; } = string.Empty;

    #endregion

    #region Server2Client

    public IMultiplayerRoom Room { get; init; } = null!;

    #endregion

    public static MultiCreatePacket CreateC2S(string name, string password, long mapID, string mapHash)
        => new() { Name = name, Password = password, MapID = mapID, MapHash = mapHash };

    public static MultiCreatePacket CreateS2C(IMultiplayerRoom room) => new() { Room = room };
}
