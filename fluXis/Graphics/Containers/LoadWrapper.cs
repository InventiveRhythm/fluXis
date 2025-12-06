using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Containers;

public partial class LoadWrapper<T> : Container, IHasLoadedValue
    where T : Drawable
{
    public Action<T> OnComplete { get; init; }
    public Func<T> LoadContent { get; set; }

    public bool Loaded { get; private set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Reload();
    }

    protected override void Update()
    {
        base.Update();
        Loaded = InternalChildren.Count >= 1 && InternalChildren[0].Alpha >= 1;
    }

    public void Reload()
    {
        Clear();
        LoadComponentAsync(LoadContent(), drawable =>
        {
            Add(drawable);
            OnComplete?.Invoke(drawable);
        });
    }
}
