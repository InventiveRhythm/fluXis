using System.Collections.Generic;
using fluXis.Online.Collections;

namespace fluXis.Online.API.Requests.Collections;

public class OwnedCollectionsRequest : APIRequest<List<Collection>>
{
    protected override string Path => $"/users/@me/collections";
}
