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

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = false;
        RelativeSizeAxes = Axes.Both;
        ScrollbarAnchor = Anchor.TopLeft;

        Child = Content = new FillFlowContainer<MapListEntry>
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Spacing = new Vector2(10),
            Direction = FillDirection.Vertical
        };
    }

    public void AddMap(MapListEntry entry)
    {
        Content.Add(entry);
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

    public void Insert(int index, MapListEntry entry)
    {
        Content.Insert(index, entry);

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
