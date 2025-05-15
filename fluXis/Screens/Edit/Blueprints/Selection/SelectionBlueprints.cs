using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Blueprints.Selection;

public partial class SelectionBlueprints<T> : Container<SelectionBlueprint<T>>
{
    public IEnumerable<SelectionBlueprint<T>> All
    {
        get
        {
            var list = back.Concat(Children).ToList();
            list.Sort(Compare);
            return list;
        }
    }

    private readonly List<SelectionBlueprint<T>> back = new();

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

        LoadComponent(blueprint);
        back.Add(blueprint);
    }

    public override bool Remove(SelectionBlueprint<T> blueprint, bool disposeImmediately)
    {
        if (!bulk)
            SortInternal();

        back.Remove(blueprint);
        return base.Remove(blueprint, disposeImmediately);
    }

    protected override void Update()
    {
        base.Update();

        var remove = Children.Where(x => !x.Visible).ToList();
        remove.ForEach(x =>
        {
            base.Remove(x, false);
            back.Add(x);
        });

        var add = back.Where(x => x.Visible).ToList();
        add.ForEach(x =>
        {
            base.Add(x);
            back.Remove(x);
        });
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

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        back.ForEach(x => x.Dispose());
    }
}
