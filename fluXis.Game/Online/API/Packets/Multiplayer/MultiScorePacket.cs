using fluXis.Game.Online.Fluxel;

namespace fluXis.Game.Online.API.Packets.Multiplayer;

public class MultiScorePacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_SCORE;

    // eventually needs to be fleshed out to do more than this
    public int Score { get; set; }

    #region Server2Client

    public long UserID { get; set; }

    #endregion

    public static MultiScorePacket CreateC2S(int score)
        => new() { Score = score };

    public static MultiScorePacket CreateS2C(long userId, int score)
        => new() { UserID = userId, Score = score };
}

