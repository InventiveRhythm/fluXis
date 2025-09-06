using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
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
    public ISelectionManager Selection { get; set; }

    public RealmMapMetadata Metadata => set.Metadata;

    public float Size
    {
        get
        {
            if (State.Value != SelectedState.Selected)
                return 80;

            var diffs = maps.Count;
            return 85 + diffs * 53 - 5;
        }
    }

    public float Position { get; set; }
    public bool Displayed { get; set; }

    public float ScrollPosition
    {
        get
        {
            var current = maps.IndexOf(Selection.CurrentMap);

            if (current < 0 || current >= maps.Count)
                return Position;

            return Position + 85 + current * (48 + 5);
        }
    }

    public float ScrollSize => 48;

    public Drawable Drawable { get; set; }

    private RealmMapSet set { get; }
    private List<RealmMap> maps { get; }

    public MapSetItem(RealmMapSet set, List<RealmMap> maps = null)
    {
        this.set = set;
        this.maps = maps ?? set.MapsSorted;
        this.maps.Sort((a, b) => MapUtils.CompareMap(a, b, MapUtils.SortingMode.Difficulty));
    }

    public Drawable CreateDrawable() => Drawable = new DrawableMapSetItem(this, set, maps)
    {
        SelectAction = Screen.Accept,
        EditAction = Screen.EditMap,
        DeleteAction = Screen.DeleteMapSet,
        ExportAction = Screen.ExportMapSet
    };

    public void Bind() => Selection.MapSetBindable.BindValueChanged(mapSetChanged, true);
    public void Unbind() => Selection.MapSetBindable.ValueChanged -= mapSetChanged;

    public void Select(bool last = false)
    {
        var map = last ? maps.Last() : maps.First();
        Selection.Select(map);
    }

    public bool Matches(object obj)
    {
        if (obj is RealmMapSet)
            return ReferenceEquals(obj, set);

        if (obj is RealmMap map)
            return maps.Any(m => ReferenceEquals(m, map));

        return false;
    }

    public bool MatchesFilter(SearchFilters filters)
    {
        var first = maps.FirstOrDefault(filters.Matches);
        return first is not null;
    }

    public bool ChangeChild(int by)
    {
        int current = maps.IndexOf(Selection.CurrentMap);
        current += by;

        if (current < 0)
            return true;

        if (current >= maps.Count)
            return true;

        Selection.Select(maps[current]);
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
