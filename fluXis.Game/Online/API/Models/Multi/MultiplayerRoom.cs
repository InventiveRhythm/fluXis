using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Multi;

public class MultiplayerRoom
{
    [JsonProperty("id")]
    public int RoomID { get; set; }

    [JsonProperty("settings")]
    public MultiplayerRoomSettings Settings { get; set; }

    [JsonProperty("host")]
    public APIUserShort Host { get; set; }

    [JsonProperty("users")]
    public List<APIUserShort> Users { get; set; }

    [JsonProperty("maps")]
    public List<APIMapShort> Maps { get; set; }
}
