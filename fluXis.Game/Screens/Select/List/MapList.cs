using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Map;
using fluXis.Game.Screens.Select.List.Items;
using fluXis.Game.UI;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class MapList : FluXisScrollContainer
{
    public new FillFlowContainer Content { get; private set; }

    [Resolved]
    private SelectScreen screen { get; set; }

    [Resolved]
    private MapStore store { get; set; }

    private List<IListItem> items { get; } = new();

    public IReadOnlyList<IListItem> Items => items;

    private Bindable<MapUtils.SortingMode> sorting { get; }

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
        ScrollbarOverlapsContent = false;

        Child = Content = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Spacing = new Vector2(10),
            Direction = FillDirection.Vertical
        };
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
        item.Store = store;
        item.Sorting = sorting;
        item.Bind();

        items.Add(item);
        Content.Add(item.CreateDrawable());

        if (bulkInserting)
            return;

        Sort();
    }

    public void Remove(IListItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Content.Remove(item.Drawable, true);
            item.Unbind();
        }
    }

    public void Sort()
    {
        items.Sort((a, b) => a.CompareTo(b));

        for (int i = 0; i < items.Count; i++)
            Content.SetLayoutPosition(items[i].Drawable, i);
    }

    public void ScrollToSelected()
    {
        var selected = items.FirstOrDefault(c => c.State.Value == SelectedState.Selected);

        if (selected != null)
            ScrollToItem(selected);
    }

    public void ScrollToItem(IListItem item)
    {
        ScrollTo(item.Drawable);
    }
}
