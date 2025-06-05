using System;
using fluXis.Database.Maps;
using fluXis.UI;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Select.List.Items;

public interface IListItem : IComparable<IListItem>
{
    Bindable<MapUtils.SortingMode> Sorting { get; set; }
    Bindable<SelectedState> State { get; }

    SelectScreen Screen { get; set; }
    ISelectionManager Selection { get; set; }

    RealmMapMetadata Metadata { get; }

    float Size { get; }
    float Position { get; set; }
    bool Displayed { get; set; }

    float ScrollPosition => Position;
    float ScrollSize => Size;

    Drawable Drawable { get; set; }
    Drawable CreateDrawable();

    void Bind();
    void Unbind();

    void Select(bool last = false);
    bool ChangeChild(int by);

    bool Matches(object obj);
    bool MatchesFilter(SearchFilters filters);
}
