using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Online.Collections;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Screens.Select.Search.Filters;

#nullable enable

public partial class SearchCollectionFilter : SearchFilterControl<Collection?>
{
    public SearchCollectionFilter(CollectionManager manager, Bindable<Collection?> collection)
        : base(LocalizationStrings.SongSelect.Collections, new Collection?[] { null }.Concat(manager.Collections).ToArray(), collection)
    {
    }

    protected override IconUsage GenerateItemIcon(Collection? item)
    {
        switch (item?.Type)
        {
            case CollectionType.Favorite:
                return Phosphor.Bold.HeartStraight;

            case CollectionType.Owned:
                return Phosphor.Bold.Playlist;

            case CollectionType.Subscribed:
                return Phosphor.Bold.GlobeHemisphereWest;

            default:
                return Phosphor.Bold.X;
        }
    }

    protected override LocalisableString GenerateItemText(Collection? item)
    {
        if (item?.Type == CollectionType.Favorite)
            return LocalizationStrings.SongSelect.CollectionFavorite;
        if (!string.IsNullOrWhiteSpace(item?.Name))
            return item.Name;

        return LocalizationStrings.SongSelect.CollectionNone;
    }
}
