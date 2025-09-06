using System;
using System.Collections.Generic;
using fluXis.Online.API.Requests.Collections;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Online.Collections;

public partial class CollectionManager : Component
{
    [Resolved]
    private IAPIClient api { get; set; }

    public List<Collection> Collections { get; private set; } = new();

    public void Fetch(Action complete)
    {
        if (!api.IsLoggedIn)
        {
            complete();
            return;
        }

        var req = new OwnedCollectionsRequest();
        req.Success += res =>
        {
            Collections.AddRange(res.Data);
            complete?.Invoke();
        };
        req.Failure += ex => complete?.Invoke();
        api.PerformRequestAsync(req);
    }
}
