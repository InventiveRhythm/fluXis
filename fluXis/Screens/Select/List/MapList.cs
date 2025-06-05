using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Map;
using fluXis.Screens.Select.List.Items;
using fluXis.UI;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Select.List;

public partial class MapList : FluXisScrollContainer, ISelectionManager
{
    public new Container Content { get; private set; }

    [Resolved]
    private SelectScreen screen { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    private Bindable<MapUtils.SortingMode> sorting { get; }
    private List<IListItem> items { get; } = new();

    private bool bulkInserting;

    public MapList(Bindable<MapUtils.SortingMode> sorting)
    {
        this.sorting = sorting;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Alpha = 0;
        Masking = false;
        RelativeSizeAxes = Axes.Both;
        ScrollbarAnchor = Anchor.TopLeft;
        ScrollbarOverlapsContent = true;
        ScrollbarMargin = 4;

        Child = Content = new Container
        {
            RelativeSizeAxes = Axes.X
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(mapChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        maps.MapBindable.ValueChanged -= mapChanged;
        base.Dispose(isDisposing);
    }

    private void mapChanged(ValueChangedEvent<RealmMap> v)
    {
        MapBindable.Value = v.NewValue;
        MapSetBindable.Value = CurrentMap.MapSet;
    }

    public void StartBulkInsert() => bulkInserting = true;

    public void EndBulkInsert()
    {
        bulkInserting = false;
        Sort();
    }

    public void Insert(IListItem item)
    {
        item.Screen = screen;
        item.Selection = this;
        item.Sorting = sorting;
        item.Bind();

        items.Add(item);
        item.CreateDrawable();

        if (bulkInserting)
            return;

        Sort();
    }

    public void Remove(IListItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);

            if (item.Drawable is not null && !Content.Remove(item.Drawable, true))
                item.Drawable.Dispose();

            item.Unbind();
        }
    }

    protected override void Update()
    {
        base.Update();

        debounce -= Time.Elapsed;

        if (!Equals(maps.CurrentMap, CurrentMap) && debounce <= 0)
            selectCurrent();

        var pos = 0f;

        for (var idx = 0; idx < items.Count; idx++)
        {
            var item = items[idx];
            item.Position = pos;

            var size = item.Size;
            pos += size + 10;

            if (item.Position + size < Current - 250 || item.Position > Current + DrawHeight + 250)
            {
                if (!item.Displayed)
                    continue;

                item.Displayed = false;
                Content.Remove(item.Drawable, false);
            }
            else if (item.Drawable.Parent == null)
            {
                Content.Add(item.Drawable);
                item.Drawable.Y = item.Position;
                item.Displayed = true;
            }
        }

        Content.Height = pos;
    }

    public void Sort()
    {
        items.Sort((a, b) => a.CompareTo(b));
    }

    // used for testing
    public void SetSorting(MapUtils.SortingMode mode)
    {
        sorting.Value = mode;
        Sort();
    }

    public void ScrollToSelected(bool smooth = true)
    {
        var selected = items.FirstOrDefault(c => c.State.Value == SelectedState.Selected);

        if (selected != null)
            ScrollToItem(selected, smooth);
    }

    public void ScrollToItem(IListItem item, bool smooth = true)
    {
        var top = item.ScrollPosition;
        var center = top + item.ScrollSize / 2;

        if (center < DisplayableContent / 2)
            ScrollTo(0, smooth);
        else if (center > ScrollableExtent + DisplayableContent / 2)
            ScrollToEnd(smooth);
        else
            ScrollTo(center - DisplayableContent / 2, smooth);
    }

    public RealmMap CurrentMap => MapBindable.Value;
    public RealmMapSet CurrentMapSet => MapSetBindable.Value;
    public Bindable<RealmMap> MapBindable { get; set; } = new();
    public Bindable<RealmMapSet> MapSetBindable { get; set; } = new();

    private double debounce;

    public void Select(RealmMap map)
    {
        MapBindable.Value = map;
        MapSetBindable.Value = map.MapSet;

        var item = items.FirstOrDefault(i => i.Matches(map));
        ScrollToItem(item);

        if (debounce <= 0)
            selectCurrent();

        debounce = 120;
    }

    private void selectCurrent()
    {
        maps.Select(CurrentMap, true);
    }
}
