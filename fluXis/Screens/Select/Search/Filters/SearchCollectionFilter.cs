using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Online.Collections;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Screens.Select.Search.Filters;

#nullable enable

public partial class SearchCollectionFilter : SearchFilterControl<Collection?>
{
    public SearchCollectionFilter(CollectionManager manager, Bindable<Collection?> collection)
        : base("Collections", new Collection?[] { null }.Concat(manager.Collections).ToArray(), collection)
    {
    }

    protected override IconUsage GenerateItemIcon(Collection? item) => FontAwesome6.Solid.Question;
    protected override LocalisableString GenerateItemText(Collection? item) => item?.Name ?? "None";
}
