using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Blueprints.Selection;

public partial class SelectionBlueprints<T> : Container<SelectionBlueprint<T>>
{
    private bool bulk;

    public void StartBulk() => bulk = true;

    public void EndBulk()
    {
        bulk = false;
        SortInternal();
    }

    public override void Add(SelectionBlueprint<T> blueprint)
    {
        if (!bulk)
            SortInternal();

        base.Add(blueprint);
    }

    public override bool Remove(SelectionBlueprint<T> blueprint, bool disposeImmediately)
    {
        if (!bulk)
            SortInternal();

        return base.Remove(blueprint, disposeImmediately);
    }

    protected override int Compare(Drawable a, Drawable b)
    {
        var aHit = (SelectionBlueprint<T>)a;
        var bHit = (SelectionBlueprint<T>)b;

        int result = bHit.FirstComparer.CompareTo(aHit.FirstComparer);
        if (result != 0) return result;

        result = bHit.SecondComparer.CompareTo(aHit.SecondComparer);
        return result != 0 ? result : CompareReverseChildID(a, b);
    }
}
