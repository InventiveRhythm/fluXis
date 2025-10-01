using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Default;
using fluXis.Skinning.Json;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Skinning.Bases;

public partial class ColorableSkinDrawable : CompositeDrawable
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    protected ICustomColorProvider ColorProvider { get; private set; }

    protected SkinJson SkinJson { get; }

    protected bool UseCustomColor { get; set; }

    public MapColor Index { get; private set; }

    public ColorableSkinDrawable(SkinJson skinJson, MapColor index)
    {
        SkinJson = skinJson;
        Index = index;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RegisterToProvider();
    }

    public void UpdateColor(int lane, int keyCount) => Schedule(() =>
    {
        if (UseCustomColor)
            return;

        if (ColorProvider != null && ColorProvider.HasColorFor(lane, keyCount, out var color))
        {
            SetColor(color);
            return;
        }

        SetColor(SkinJson?.GetLaneColor(lane, keyCount) ?? Colour4.White);
    });

    public virtual void SetColor(Colour4 color) { }
    public virtual void UpdateColor(MapColor index, Colour4 color) => SetColor(color);

    protected virtual void RegisterToProvider() => ColorProvider?.Register(this, Index);
    protected virtual void UnregisterFromProvider() => ColorProvider?.Unregister(this, Index);

    protected Colour4 GetIndexOrFallback(int index, Colour4 fallback)
    {
        var col = ColorProvider?.GetColor(index, fallback);

        if (col == null)
            return fallback;

        return col.Value;
    }

    protected override void Dispose(bool isDisposing)
    {
        UnregisterFromProvider();
        base.Dispose(isDisposing);
    }
}
