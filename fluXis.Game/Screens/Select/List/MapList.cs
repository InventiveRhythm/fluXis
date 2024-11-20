using System;
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

namespace fluXis.Game.Screens.Select.List;

public partial class MapList : FluXisScrollContainer
{
    public new Container Content { get; private set; }

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

        Child = Content = new Container
        {
            RelativeSizeAxes = Axes.X
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

        var pos = 0f;

        for (var idx = 0; idx < items.Count; idx++)
        {
            var item = items[idx];
            item.Position = pos;

            var size = item.Size;
            pos += size + 10;

            if (item.Position + size < Current - 250 || item.Position > Current + DrawHeight + 250)
                Content.Remove(item.Drawable, false);
            else if (item.Drawable.Parent == null)
            {
                Content.Add(item.Drawable);
                item.Drawable.Y = item.Position;
            }
        }

        Content.Height = pos;
    }

    public void Sort()
    {
        items.Sort((a, b) => a.CompareTo(b));
    }

    public void ScrollToSelected()
    {
        var selected = items.FirstOrDefault(c => c.State.Value == SelectedState.Selected);

        if (selected != null)
            ScrollToItem(selected);
    }

    public void ScrollToItem(IListItem item)
    {
        var pos1 = item.Position;
        var pos2 = item.Position + item.Size;

        var min = Math.Min(pos1, pos2);
        var max = Math.Max(pos1, pos2);

        if (min < Current || (min > Current && pos2 > AvailableContent))
            ScrollTo(min);
        else if (max > Current + DisplayableContent)
            ScrollTo(max - DisplayableContent);
    }
}
