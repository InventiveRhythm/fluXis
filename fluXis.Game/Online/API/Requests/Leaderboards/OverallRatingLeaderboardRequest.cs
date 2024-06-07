using System.Collections.Generic;
using fluXis.Shared.Components.Users;

namespace fluXis.Game.Online.API.Requests.Leaderboards;

public class OverallRatingLeaderboardRequest : APIRequest<List<APIUser>>
{
    protected override string Path => "/leaderboards/overall";
}
