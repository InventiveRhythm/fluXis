using System.Collections.Generic;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Multi;
using fluXis.Shared.Components.Users;

namespace fluXis.Game.Online.API.Models.Multi;

public class MultiplayerRoom : IMultiplayerRoom
{
    public long RoomID { get; init; }
    public IMultiplayerRoomSettings Settings { get; set; } = new MultiplayerRoomSettings();
    public IAPIUserShort Host { get; set; } = null!;
    public List<IAPIUserShort> Users { get; set; } = null!;
    public List<IAPIMapShort> Maps { get; set; } = null!;
}
