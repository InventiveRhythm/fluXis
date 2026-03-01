using System;
using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Graphics.Shaders.Bloom;

public class BloomShaderStep : ShaderStep<BloomShaderStep.BlurParameters>
{
    protected override string FragmentShader => "Blur";
    protected string ComposeFragmentShader => "BlurCompose";
    private IShader blurComposeShader;
    public override ShaderType Type => ShaderType.Bloom;

    public override int Passes => 1;

    private int kernelRadius;
    private float sigma;
    private float radians = 0;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        Radius = kernelRadius,
        Sigma = sigma,
        TexSize = current.Size,
        Direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians))
    };

    public override void LoadShader(ShaderManager shaders)
    {
        base.LoadShader(shaders);
        blurComposeShader = shaders.Load(VertexShader, ComposeFragmentShader);
    }

    private IFrameBuffer buffer;

    public void DrawCombined(IRenderer renderer, IFrameBuffer blurred, IFrameBuffer current)
    {

    }

    public override void DrawBuffer(IRenderer renderer, IFrameBuffer current, IFrameBuffer target)
    {

        sigma = 20 * Strength;
        kernelRadius = Blur.KernelSize(sigma);
        DrawColor = Colour4.White;

        buffer ??= renderer.CreateFrameBuffer();
        buffer.Size = current.Size;

        var name = typeof(BlurParameters).Name;
        Shader.BindUniformBlock($"m_{name}", ParameterBuffer);

        target.Unbind();
        buffer.Bind();
        Shader.Bind();

        DrawFrameBuffer(renderer, current);

        Shader.Unbind();
        buffer.Unbind();
        target.Bind();

        blurComposeShader.Bind();
        renderer.BindTexture(buffer.Texture, 1);
        DrawFrameBuffer(renderer, current);
        blurComposeShader.Unbind();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct BlurParameters
    {
        public UniformVector2 TexSize;
        public UniformInt Radius;
        public UniformFloat Sigma;
        public UniformVector2 Direction;
        private readonly UniformPadding8 pad1;
    }
}
