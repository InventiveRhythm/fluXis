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

    public void Reload()
    {
        Clear();
        LoadComponentAsync(LoadContent(), drawable =>
        {
            Add(drawable);
            Loaded = true;
            OnComplete?.Invoke(drawable);
        });
    }
}
