using System.Collections.Generic;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Models.Users;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Game.Replays;

public class Replay
{
    public int PlayerID { get; set; } = -1;
    public List<ReplayFrame> Frames { get; set; } = new();

    [JsonIgnore]
    [CanBeNull]
    public APIUserShort Player
    {
        get
        {
            if (PlayerID == -1) return APIUserShort.Default;
            if (PlayerID == 0) return APIUserShort.AutoPlay;

            var user = UserCache.GetUser(PlayerID);
            return user;
        }
    }
}
