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
    APIUserShort Host { get; set; }

    [JsonProperty("users")]
    List<APIUserShort> Users { get; set; }

    [JsonProperty("maps")]
    List<IAPIMapShort> Maps { get; set; }
}
