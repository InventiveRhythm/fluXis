using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Multi;

[JsonObject(MemberSerialization.OptIn)]
public interface IMultiplayerRoom
{
    [JsonProperty("id")]
    long RoomID { get; init; }

    [JsonProperty("settings")]
    IMultiplayerRoomSettings Settings { get; init; }

    [JsonProperty("host")]
    APIUser Host { get; set; }

    [JsonProperty("participants")]
    List<IMultiplayerParticipant> Participants { get; init; }

    [JsonProperty("map")]
    APIMap Map { get; set; }
}
