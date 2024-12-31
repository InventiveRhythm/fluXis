using fluXis.Online;
using fluXis.Online.API.Models.Users;
using fluXis.Replays;

namespace fluXis.Utils.Extensions;

public static class ReplayExtensions
{
    public static APIUser GetPlayer(this Replay replay, UserCache users)
    {
        if (replay.PlayerID == -1 || users is null)
            return APIUser.Default;

        if (replay.PlayerID == 0)
            return APIUser.AutoPlay;

        var user = users.Get(replay.PlayerID);
        return user;
    }
}
