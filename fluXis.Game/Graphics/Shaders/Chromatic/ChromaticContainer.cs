using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Layout;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Chromatic;

public partial class ChromaticContainer<T> : Container<T>, IBufferedDrawable where T : Drawable
{
    public float Strength
    {
        get => strength;
        set
        {
            if (strength == value) return;

            strength = value;
            ForceRedraw();
        }
    }

    protected override bool RequiresChildrenUpdate => base.RequiresChildrenUpdate && childrenUpdateVersion != updateVersion;

    public IShader TextureShader { get; private set; }
    public Color4 BackgroundColour => Colour4.Transparent;
    public DrawColourInfo? FrameBufferDrawColour => base.DrawColourInfo;
    public Vector2 FrameBufferScale => Vector2.One;

    private ChromaticDrawData sharedData { get; } = new(null, false, true);
    private IShader chromaticShader { get; set; }

    private float strength = 1f;

    private long updateVersion;
    private long childrenUpdateVersion = -1;

    [BackgroundDependencyLoader]
    private void load(ShaderManager shaders)
    {
        TextureShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE);
        chromaticShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, "ChromaticAberration");
    }

    public void ForceRedraw() => Invalidate(Invalidation.DrawNode);

    protected override DrawNode CreateDrawNode() => new ChromaticContainerDrawNode(this, sharedData);

    public override bool UpdateSubTreeMasking(Drawable source, RectangleF maskingBounds)
    {
        bool result = base.UpdateSubTreeMasking(source, maskingBounds);

        childrenUpdateVersion = updateVersion;
        return result;
    }

    protected override RectangleF ComputeChildMaskingBounds(RectangleF maskingBounds) => ScreenSpaceDrawQuad.AABBFloat;

    protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
    {
        bool result = base.OnInvalidate(invalidation, source);

        if ((invalidation & Invalidation.DrawNode) <= 0) return result;

        updateVersion++;
        return true;
    }

    protected override void Update()
    {
        base.Update();
        ForceRedraw();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        sharedData.Dispose();
    }
}
