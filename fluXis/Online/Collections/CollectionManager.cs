using System;
using System.Collections.Generic;
using fluXis.Online.API.Requests.Collections;
using fluXis.Online.Fluxel;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Online.Collections;

public partial class CollectionManager : Component
{
    [Resolved]
    private IAPIClient api { get; set; }

    public List<Collection> Collections { get; private set; } = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        api.OnCollectionUpdated += updateCollection;
    }

    private void updateCollection(string id, List<CollectionItem> add, List<CollectionItem> change, List<string> remove)
    {
        Logger.Log($"updating {id}. {add.Serialize()} {change.Serialize()} {remove.Serialize()}", LoggingTarget.Network, LogLevel.Debug);

        var collection = Collections.Find(x => x.ID == id);
        if (collection is null) return;

        collection.Items.AddRange(add);

        foreach (var item in change)
        {
            var idx = collection.Items.FindIndex(x => x.ID == item.ID);
            if (idx < 0) continue;

            collection.Items[idx] = item;
        }

        remove.ForEach(x => collection.Items.RemoveAll(i => i.ID == x));
    }

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
        req.Failure += _ => complete?.Invoke();
        api.PerformRequestAsync(req);
    }
}
