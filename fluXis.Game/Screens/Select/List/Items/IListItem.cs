using System;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.UI;
using fluXis.Game.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Select.List.Items;

public interface IListItem : IComparable<IListItem>
{
    Bindable<MapUtils.SortingMode> Sorting { get; set; }
    Bindable<SelectedState> State { get; }

    SelectScreen Screen { get; set; }
    MapStore Store { get; set; }

    RealmMapMetadata Metadata { get; }

    Drawable Drawable { get; set; }
    Drawable CreateDrawable();

    void Bind();
    void Unbind();

    void Select(bool last = false);
    bool ChangeChild(int by);

    bool Matches(object obj);
    bool MatchesFilter(SearchFilters filters);
}
