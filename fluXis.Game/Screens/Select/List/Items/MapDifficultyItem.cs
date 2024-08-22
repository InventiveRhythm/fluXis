using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Screens.Select.List.Drawables.Difficulty;
using fluXis.Game.UI;
using fluXis.Game.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Select.List.Items;

public class MapDifficultyItem : IListItem
{
    public Bindable<MapUtils.SortingMode> Sorting { get; set; }
    public Bindable<SelectedState> State { get; } = new();

    public SelectScreen Screen { get; set; }
    public MapStore Store { get; set; }

    public RealmMapMetadata Metadata => map.Metadata;

    public float Size => 80;
    public float Position { get; set; }

    public Drawable Drawable { get; set; }

    private RealmMap map { get; }

    public MapDifficultyItem(RealmMap map)
    {
        this.map = map;
    }

    public Drawable CreateDrawable()
    {
        return Drawable = new DrawableDifficultyItem(this, map)
        {
            SelectAction = Screen.Accept,
            EditAction = Screen.EditMap,
            DeleteAction = Screen.DeleteMapSet,
            ExportAction = Screen.ExportMapSet
        };
    }

    public void Bind()
    {
        Store.MapBindable.BindValueChanged(mapChanged, true);
    }

    public void Unbind()
    {
        Store.MapBindable.ValueChanged -= mapChanged;
    }

    public void Select(bool last = false) => Store.Select(map, true);

    public bool Matches(object obj)
    {
        return obj switch
        {
            RealmMapSet => ReferenceEquals(obj, map.MapSet),
            RealmMap => ReferenceEquals(obj, map),
            _ => false
        };
    }

    public bool MatchesFilter(SearchFilters filters) => filters.Matches(map);

    public bool ChangeChild(int by) => true;

    private void mapChanged(ValueChangedEvent<RealmMap> vce)
        => State.Value = ReferenceEquals(vce.NewValue, map) ? SelectedState.Selected : SelectedState.Deselected;

    public int CompareTo(MapDifficultyItem other) => MapUtils.CompareMap(map, other.map, Sorting.Value, Screen.SortInverse);

    public int CompareTo(IListItem other)
    {
        if (other is not MapDifficultyItem o)
            return -1;

        return CompareTo(o);
    }
}
