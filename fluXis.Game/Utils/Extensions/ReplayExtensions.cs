using fluXis.Game.Online;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Replays;

namespace fluXis.Game.Utils.Extensions;

public static class ReplayExtensions
{
    public static APIUser GetPlayer(this Replay replay, UserCache users)
    {
        if (replay.PlayerID == -1)
            return APIUser.Default;

        if (replay.PlayerID == 0)
            return APIUser.AutoPlay;

        var user = users.Get(replay.PlayerID);
        return user;
    }
}
