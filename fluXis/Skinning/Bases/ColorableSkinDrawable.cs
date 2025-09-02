using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
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
        ColorProvider.Register(this);
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

    /// <summary>
    /// Not to be confused with <see cref="TransformableExtensions.FadeColour{T}(T, osu.Framework.Graphics.Colour.ColourInfo, double, Easing)" />
    /// </summary>
    /// <param name="color"></param>
    public virtual void FadeColor(Colour4 color, double duration = 0, Easing easing = Easing.None) { }

    protected Colour4 GetIndexOrFallback(int index, Colour4 fallback)
    {
        var col = ColorProvider?.GetColor(index, fallback);

        if (col == null)
            return fallback;

        return col.Value;
    }

    protected override void Dispose(bool isDisposing)
    {
        ColorProvider.Unregister(this);
        base.Dispose(isDisposing);
    }
}
