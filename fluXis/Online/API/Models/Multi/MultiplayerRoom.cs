using System.Collections.Generic;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;

namespace fluXis.Online.API.Models.Multi;

public class MultiplayerRoom : IMultiplayerRoom
{
    public long RoomID { get; init; }
    public IMultiplayerRoomSettings Settings { get; init; } = new MultiplayerRoomSettings();
    public APIUser Host { get; set; } = null!;
    public List<IMultiplayerParticipant> Participants { get; init; } = new();
    public APIMap Map { get; set; } = null!;

    public List<ScoreInfo> Scores { get; set; } = new();
}
