using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Multi;

public interface IMultiplayerRoom
{
    [JsonProperty("id")]
    long RoomID { get; init; }

    [JsonProperty("settings")]
    IMultiplayerRoomSettings Settings { get; set; }

    [JsonProperty("host")]
    IAPIUserShort Host { get; set; }

    [JsonProperty("users")]
    List<IAPIUserShort> Users { get; set; }

    [JsonProperty("maps")]
    List<IAPIMapShort> Maps { get; set; }
}
