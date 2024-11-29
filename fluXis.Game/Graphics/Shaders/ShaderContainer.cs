using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Layout;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders;

public abstract partial class ShaderContainer : Container, IBufferedDrawable
{
    protected virtual string VertexShader => VertexShaderDescriptor.TEXTURE_2;
    protected abstract string FragmentShader { get; }
    public abstract ShaderType Type { get; }
    protected abstract DrawNode CreateShaderDrawNode();

    public ShaderTransformHandler TransformHandler { get; }

    private float strength;
    private float strength2;
    private float strength3;

    public float Strength
    {
        get => strength;
        set
        {
            if (value == strength)
                return;

            strength = value;
            Invalidate(Invalidation.DrawNode);
        }
    }

    public float Strength2
    {
        get => strength2;
        set
        {
            if (value == strength2)
                return;

            strength2 = value;
            Invalidate(Invalidation.DrawNode);
        }
    }

    public float Strength3
    {
        get => strength3;
        set
        {
            if (value == strength3)
                return;

            strength3 = value;
            Invalidate(Invalidation.DrawNode);
        }
    }

    public bool DrawOriginal { get; set; }
    public ColourInfo EffectColour { get; set; } = Color4.White;
    public BlendingParameters EffectBlending { get; set; } = BlendingParameters.Inherit;
    public EffectPlacement EffectPlacement { get; set; }
    public Color4 BackgroundColour { get; set; } = new(0, 0, 0, 0);
    public Vector2 FrameBufferScale { get; set; } = Vector2.One;

    public BlendingParameters DrawEffectBlending
    {
        get
        {
            BlendingParameters blending = EffectBlending;

            blending.CopyFromParent(Blending);
            blending.ApplyDefaultToInherited();

            return blending;
        }
    }

    protected override bool RequiresChildrenUpdate => base.RequiresChildrenUpdate && childrenUpdateVersion != updateVersion;

    public IShader TextureShader { get; private set; }
    public DrawColourInfo? FrameBufferDrawColour => base.DrawColourInfo;

    protected BufferedDrawNodeSharedData SharedData { get; } = new(2, null, false, true);
    private IShader shader { get; set; }

    private long updateVersion;
    private long childrenUpdateVersion = -1;

    protected ShaderContainer()
    {
        TransformHandler = new ShaderTransformHandler(this);
    }

    [BackgroundDependencyLoader]
    private void load(ShaderManager shaders)
    {
        TextureShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE);
        shader = shaders.Load(VertexShader, FragmentShader);
    }

    protected void ForceRedraw() => Invalidate(Invalidation.DrawNode);

    protected override DrawNode CreateDrawNode() => CreateShaderDrawNode();

    public override bool UpdateSubTreeMasking()
    {
        bool result = base.UpdateSubTreeMasking();

        childrenUpdateVersion = updateVersion;
        return result;
    }

    protected override RectangleF ComputeChildMaskingBounds() => ScreenSpaceDrawQuad.AABBFloat;

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
        SharedData.Dispose();
    }
}
