using fluXis.UI;
using osu.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Containers;

#nullable enable

public partial class SelectionCycleContainer<T> : FillFlowContainer<T>
    where T : Drawable, IStateful<SelectedState>
{
    public T? Current => (idx >= 0 && idx < Count) ? this[idx.Value] : null;

    private int? idx;

    public void Next()
    {
        if (!idx.HasValue || idx == Count - 1)
            set(0);
        else
            set(idx.Value + 1);
    }

    public void Previous()
    {
        if (idx is null or 0)
            set(Count - 1);
        else
            set(idx.Value - 1);
    }

    public void Select(T item)
    {
        int index = IndexOf(item);

        if (index < 0)
            set(null);
        else
            set(index);
    }

    public void Deselect() => set(null);

    private void set(int? index)
    {
        if (idx == index)
            return;

        if (idx.HasValue)
            this[idx.Value].State = SelectedState.Deselected;

        idx = index;

        if (idx.HasValue)
            this[idx.Value].State = SelectedState.Selected;
    }

    public override void Add(T drawable)
    {
        base.Add(drawable);

        drawable.StateChanged += state =>
        {
            if (state == SelectedState.Selected)
                Select(drawable);
            else
                Deselect();
        };
    }
}
