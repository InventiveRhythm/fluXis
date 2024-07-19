using System.Linq;
using fluXis.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class MapList : FluXisScrollContainer
{
    public new FillFlowContainer<MapListEntry> Content { get; private set; }

    private bool bulkInserting;

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = false;
        RelativeSizeAxes = Axes.Both;
        ScrollbarAnchor = Anchor.TopLeft;
        ScrollbarOverlapsContent = false;

        Child = Content = new FillFlowContainer<MapListEntry>
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Spacing = new Vector2(10),
            Direction = FillDirection.Vertical
        };
    }

    public override bool Remove(Drawable drawable, bool disposeImmediately)
    {
        if (drawable is MapListEntry entry)
        {
            Content.Remove(entry, true);
            return true;
        }

        return base.Remove(drawable, disposeImmediately);
    }

    public void StartBulkInsert() => bulkInserting = true;

    public void Insert(MapListEntry entry)
    {
        Content.Add(entry);

        if (bulkInserting)
            return;

        Sort();
    }

    public void EndBulkInsert()
    {
        bulkInserting = false;
        Sort();
    }

    public void Sort()
    {
        var sorted = Content.Children.ToList();
        sorted.Sort((a, b) => a.CompareTo(b));

        for (int i = 0; i < sorted.Count; i++)
            Content.SetLayoutPosition(sorted[i], i);
    }

    public void ScrollToSelected()
    {
        var selected = Content.Children.FirstOrDefault(c => c.Selected);
        if (selected != null) ScrollTo(selected);
    }
}
