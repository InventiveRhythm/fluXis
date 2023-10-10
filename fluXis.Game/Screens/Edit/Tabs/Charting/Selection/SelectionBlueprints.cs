using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Selection;

public partial class SelectionBlueprints : Container<SelectionBlueprint>
{
    public override void Add(SelectionBlueprint blueprint)
    {
        SortInternal();
        base.Add(blueprint);
    }

    public override bool Remove(SelectionBlueprint blueprint, bool disposeImmediately)
    {
        SortInternal();
        return base.Remove(blueprint, disposeImmediately);
    }

    protected override int Compare(Drawable a, Drawable b)
    {
        var aHit = (SelectionBlueprint)a;
        var bHit = (SelectionBlueprint)b;

        int result = bHit.FirstComparer.CompareTo(aHit.FirstComparer);
        if (result != 0) return result;

        result = bHit.SecondComparer.CompareTo(aHit.SecondComparer);
        return result != 0 ? result : CompareReverseChildID(a, b);
    }
}
