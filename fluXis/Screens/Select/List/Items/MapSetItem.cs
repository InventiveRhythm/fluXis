using System;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Screens.Select.List.Drawables.MapSet;
using fluXis.UI;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Select.List.Items;

public class MapSetItem : IListItem, IComparable<MapSetItem>
{
    public Bindable<MapUtils.SortingMode> Sorting { get; set; }
    public Bindable<SelectedState> State { get; } = new();

    public SelectScreen Screen { get; set; }
    public MapStore Store { get; set; }

    public RealmMapMetadata Metadata => set.Metadata;

    public float Size
    {
        get
        {
            if (State.Value != SelectedState.Selected)
                return 80;

            var diffs = set.Maps.Count;

            return 85 + diffs * 53 - 5;
        }
    }

    public float Position { get; set; }
    public bool Displayed { get; set; }

    public float ScrollPosition
    {
        get
        {
            var sorted = set.MapsSorted;
            var current = sorted.IndexOf(Store.CurrentMap);

            if (current < 0 || current >= sorted.Count)
                return Position;

            return Position + 85 + current * (48 + 5);
        }
    }

    public float ScrollSize => 48;

    public Drawable Drawable { get; set; }

    private RealmMapSet set { get; }

    public MapSetItem(RealmMapSet set)
    {
        this.set = set;
    }

    public Drawable CreateDrawable() => Drawable = new DrawableMapSetItem(this, set)
    {
        SelectAction = Screen.Accept,
        EditAction = Screen.EditMap,
        DeleteAction = Screen.DeleteMapSet,
        ExportAction = Screen.ExportMapSet
    };

    public void Bind() => Store.MapSetBindable.BindValueChanged(mapSetChanged, true);
    public void Unbind() => Store.MapSetBindable.ValueChanged -= mapSetChanged;

    public void Select(bool last = false)
    {
        var map = last ? set.HighestDifficulty : set.LowestDifficulty;
        Store.Select(map, true);
    }

    public bool Matches(object obj)
    {
        if (obj is RealmMapSet)
            return ReferenceEquals(obj, set);

        if (obj is RealmMap map)
            return set.Maps.Any(m => ReferenceEquals(m, map));

        return false;
    }

    public bool MatchesFilter(SearchFilters filters)
    {
        var first = set.Maps.FirstOrDefault(filters.Matches);
        return first is not null;
    }

    public bool ChangeChild(int by)
    {
        var maps = set.MapsSorted;

        int current = maps.IndexOf(Store.CurrentMap);
        current += by;

        if (current < 0)
            return true;

        if (current >= maps.Count)
            return true;

        Store.Select(maps[current], true);
        return false;
    }

    private void mapSetChanged(ValueChangedEvent<RealmMapSet> e)
        => State.Value = ReferenceEquals(e.NewValue, set) ? SelectedState.Selected : SelectedState.Deselected;

    public int CompareTo(MapSetItem other) => MapUtils.CompareSets(set, other.set, Sorting.Value, Screen.SortInverse);

    public int CompareTo(IListItem other)
    {
        if (other is not MapSetItem o)
            return -1;

        return CompareTo(o);
    }
}
