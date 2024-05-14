using fluXis.Game.Skinning.Json;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Skinning.Default;

public partial class DefaultSkinDrawable : CompositeDrawable
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    protected ICustomColorProvider ColorProvider { get; private set; }

    protected SkinJson SkinJson { get; }

    protected bool UseCutomColor { get; set; }

    public DefaultSkinDrawable(SkinJson skinJson)
    {
        SkinJson = skinJson;
    }

    public void UpdateColor(int lane, int keyCount) => Schedule(() =>
    {
        if (UseCutomColor)
            return;

        if (ColorProvider != null && ColorProvider.HasColorFor(lane, keyCount, out var color))
        {
            SetColor(color);
            return;
        }

        SetColor(SkinJson?.GetLaneColor(lane, keyCount) ?? Colour4.White);
    });

    protected virtual void SetColor(Colour4 color) { }

    protected Colour4 GetIndexOrFallback(int index, Colour4 fallback)
    {
        var col = ColorProvider?.GetColor(index, fallback);

        if (col == null)
            return fallback;

        return col.Value;
    }
}
