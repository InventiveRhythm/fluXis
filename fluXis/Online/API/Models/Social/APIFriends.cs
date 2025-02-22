using System.Collections.Generic;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Social;

public class APIFriends
{
    [JsonProperty("users")]
    public List<APIUser> Users { get; set; }

    [JsonProperty("lobbies")]
    public List<MultiplayerRoom> Rooms { get; set; }
}
