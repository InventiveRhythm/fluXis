using System.Collections.Generic;
using fluXis.Online.API.Models.Users;

namespace fluXis.Online.API.Requests.Leaderboards;

public class OverallRatingLeaderboardRequest : APIRequest<List<APIUser>>
{
    protected override string Path => "/leaderboards/overall";
}
