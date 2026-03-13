using System;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Timing;

namespace fluXis.Graphics.Shaders;

public abstract class ShaderStep : IHasStrength
{
    protected virtual string VertexShader => VertexShaderDescriptor.TEXTURE_2;
    protected abstract string FragmentShader { get; }

    protected IShader Shader { get; private set; }

    public abstract ShaderType Type { get; }
    public ShaderTransformHandler TransformHandler { get; }

    public float Strength { get; set; }
    public float Strength2 { get; set; }
    public float Strength3 { get; set; }

    public virtual bool ShouldRender => Strength > 0 || Strength2 > 0 || Strength3 > 0;
    public virtual int Passes => 1;

    protected SRGBColour DrawColor { get; set; } = Colour4.White;

    public FrameTimeInfo Time { get; set; }
    public IFrameBuffer TargetBuffer { get; set; }
    public int CurrentPass { get; set; }

    protected ShaderStep()
    {
        TransformHandler = new ShaderTransformHandler(this);
    }

    public virtual void LoadShader(ShaderManager shaders)
    {
        Shader = shaders.Load(VertexShader, FragmentShader);
    }

    public virtual void EnsureParameters(IRenderer renderer) { }
    public abstract void UpdateParameters(IFrameBuffer current);

    public virtual void DrawBuffer(IRenderer renderer, IFrameBuffer current, IFrameBuffer target)
    {
    }
}

public abstract class ShaderStep<T> : ShaderStep
    where T : unmanaged, IEquatable<T>
{
    protected IUniformBuffer<T> ParameterBuffer { get; set; }

    public override void EnsureParameters(IRenderer renderer) => ParameterBuffer ??= renderer.CreateUniformBuffer<T>();

    public override void DrawBuffer(IRenderer renderer, IFrameBuffer current, IFrameBuffer target)
    {
        var name = typeof(T).Name;
        Shader.BindUniformBlock($"m_{name}", ParameterBuffer);
        Shader.Bind();
        DrawFrameBuffer(renderer, current);
        Shader.Unbind();
    }

    protected void DrawFrameBuffer(IRenderer renderer, IFrameBuffer buffer)
        => renderer.DrawFrameBuffer(buffer, new RectangleF(0, 0, buffer.Texture.Width, buffer.Texture.Height), ColourInfo.SingleColour(DrawColor));
}
