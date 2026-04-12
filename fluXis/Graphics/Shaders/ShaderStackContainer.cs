using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Shaders.Steps;
using fluXis.Map.Structures.Events;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Layout;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Graphics.Shaders;

public partial class ShaderStackContainer : CompositeDrawable, IBufferedDrawable
{
    [Resolved]
    private ShaderManager manager { get; set; }

    private readonly List<ShaderStep> shaders = new();

    public IEnumerable<ShaderTransformHandler> TransformHandlers => shaders.Select(x => x.TransformHandler);
    public IReadOnlyList<ShaderType> ShaderTypes => shaders.DistinctBy(x => x.Type).Select(x => x.Type).ToList();

    public IShader TextureShader { get; private set; }
    public BufferedDrawNodeSharedData SharedData { get; } = new(3, null, false, true);

    public Color4 BackgroundColour => new(0, 0, 0, 0);
    public DrawColourInfo? FrameBufferDrawColour => base.DrawColourInfo;
    public Vector2 FrameBufferScale => Vector2.One;

    private long updateVersion;
    private long childrenUpdateVersion = -1;

    protected override bool RequiresChildrenUpdate => base.RequiresChildrenUpdate && childrenUpdateVersion != updateVersion;

    public ShaderStackContainer()
    {
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        TextureShader = manager.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE);
        shaders.ForEach(x => x.LoadShader(manager));
    }

    protected override DrawNode CreateDrawNode()
        => new ShaderStackDrawNode(this, new CompositeDrawableDrawNode(this), SharedData);

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
        Invalidate(Invalidation.DrawNode);
        base.Update();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        SharedData.Dispose();
    }

    public ShaderTransformHandler AddShader(ShaderStep shader)
    {
        if (manager != null)
            shader.LoadShader(manager);

        shaders.Add(shader);
        return shader.TransformHandler;
    }

    public ShaderStackContainer AddContent(Drawable[] content)
    {
        InternalChildren = content;
        return this;
    }

    public IEnumerable<Drawable> RemoveContent()
    {
        IEnumerable<Drawable> children = InternalChildren.ToArray();
        ClearInternal(false);
        return children;
    }

    public ShaderStep GetShader(ShaderType type)
        => shaders.FirstOrDefault(s => s.Type == type);

    [CanBeNull]
    public static ShaderStep CreateForType(ShaderType type) => type switch
    {
        ShaderType.Bloom => new BloomShaderStep(),
        ShaderType.Greyscale => new GreyscaleShaderStep(),
        ShaderType.Invert => new InvertShaderStep(),
        ShaderType.Chromatic => new ChromaticShaderStep(),
        ShaderType.Mosaic => new MosaicShaderStep(),
        ShaderType.Noise => new NoiseShaderStep(),
        ShaderType.Vignette => new VignetteShaderStep(),
        ShaderType.Retro => new RetroShaderStep(),
        ShaderType.HueShift => new HueShiftShaderStep(),
        ShaderType.Glitch => new GlitchShaderStep(),
        ShaderType.SplitScreen => new SplitScreenShaderStep(),
        ShaderType.FishEye => new FishEyeShaderStep(),
        ShaderType.Reflections => new ReflectionsShaderStep(),
        _ => null
    };

    private class ShaderStackDrawNode : BufferedDrawNode, ICompositeDrawNode
    {
        protected new ShaderStackContainer Source => (ShaderStackContainer)base.Source;
        protected new CompositeDrawableDrawNode Child => (CompositeDrawableDrawNode)base.Child;

        public List<DrawNode> Children
        {
            get => Child.Children;
            set => Child.Children = value;
        }

        public bool AddChildDrawNodes => RequiresRedraw;

        private long updateVersion;

        public ShaderStackDrawNode(IBufferedDrawable source, DrawNode child, BufferedDrawNodeSharedData sharedData)
            : base(source, child, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();
            updateVersion = Source.updateVersion;
        }

        protected override long GetDrawVersion() => updateVersion;

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            foreach (var shader in Source.shaders)
            {
                shader.Time = Source.Time;
                shader.EnsureParameters(renderer);

                if (!shader.ShouldRender)
                    continue;

                for (int i = 0; i < shader.Passes; i++)
                {
                    IFrameBuffer current = SharedData.CurrentEffectBuffer;
                    IFrameBuffer target = SharedData.GetNextEffectBuffer();
                    shader.TargetBuffer = target;
                    shader.CurrentPass = i + 1;

                    shader.UpdateParameters(current);
                    renderer.SetBlend(BlendingParameters.None);

                    using (BindFrameBuffer(target))
                    {
                        shader.DrawBuffer(renderer, current, target);
                    }
                }
            }
        }

        protected override void DrawContents(IRenderer renderer)
        {
            var blend = BlendingParameters.Inherit;
            blend.CopyFromParent(Source.Blending);
            blend.ApplyDefaultToInherited();
            renderer.SetBlend(blend);

            ColourInfo finalEffectColour = DrawColourInfo.Colour;
            finalEffectColour.ApplyChild(Color4.White);

            renderer.DrawFrameBuffer(SharedData.CurrentEffectBuffer, DrawRectangle, finalEffectColour);
        }
    }
}
