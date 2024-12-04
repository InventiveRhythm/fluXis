using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Scoring;

namespace fluXis.Game.Online.API.Models.Multi;

public class MultiplayerRoom : IMultiplayerRoom
{
    public long RoomID { get; init; }
    public IMultiplayerRoomSettings Settings { get; init; } = new MultiplayerRoomSettings();
    public APIUser Host { get; set; } = null!;
    public List<IMultiplayerParticipant> Participants { get; init; } = new();
    public APIMap Map { get; set; } = null!;

    public List<ScoreInfo> Scores { get; set; } = new();
}
