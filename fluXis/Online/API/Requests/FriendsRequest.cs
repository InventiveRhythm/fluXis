using System.Collections.Generic;
using fluXis.Online.API.Models.Users;

namespace fluXis.Online.API.Requests;

public class FriendsRequest : APIRequest<List<APIUser>>
{
    protected override string Path => "/friends";
}
