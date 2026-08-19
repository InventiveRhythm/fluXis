using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Containers;

public partial class ReverseFillFlowContainer<T> : FillFlowContainer<T>
    where T : Drawable
{
    protected override int Compare(Drawable x, Drawable y) => CompareReverseChildID(x, y);
}
