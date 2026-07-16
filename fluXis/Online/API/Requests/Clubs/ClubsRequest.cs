using System.Collections.Generic;
using fluXis.Online.API.Models.Clubs;

namespace fluXis.Online.API.Requests.Clubs;

public class ClubsRequest : APIRequest<List<APIClub>>
{
    protected override string Path => "/clubs";
}
