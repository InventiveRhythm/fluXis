using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Graphics.Shaders.Steps;

public class BloomShaderStep : ShaderStep<BloomShaderStep.BlurParameters>
{
    protected override string FragmentShader => "Blur";
    protected string ComposeFragmentShader => "BlurCompose";
    private IShader blurComposeShader;
    private IUniformBuffer<BlurComposeParameters> composeParameterBuffer;
    public override ShaderType Type => ShaderType.Bloom;

    public override int Passes => 1;

    private int kernelRadius;
    private float sigma;
    private Vector2 direction;

    public override void EnsureParameters(IRenderer renderer)
    {
        ParameterBuffer ??= renderer.CreateUniformBuffer<BlurParameters>();
        composeParameterBuffer ??= renderer.CreateUniformBuffer<BlurComposeParameters>();
    }

    public override void UpdateParameters(IFrameBuffer current)
    {
        ParameterBuffer.Data = ParameterBuffer.Data with
        {
            Radius = kernelRadius,
            Sigma = sigma,
            TexSize = current.Size,
            Direction = direction
        };
        composeParameterBuffer.Data = composeParameterBuffer.Data with
        {
            Strength = Strength
        };
    }

    public override void LoadShader(ShaderManager shaders)
    {
        base.LoadShader(shaders);
        blurComposeShader = shaders.Load(VertexShader, ComposeFragmentShader);
    }

    private IFrameBuffer buffer;
    private IFrameBuffer buffer2;

    private void drawBlurPass(IRenderer renderer, IFrameBuffer src, IFrameBuffer dst, Vector2 blurDirection)
    {
        direction = blurDirection;
        UpdateParameters(src);
        Shader.BindUniformBlock($"m_{nameof(BlurParameters)}", ParameterBuffer);
        dst.Bind();
        Shader.Bind();

        DrawFrameBuffer(renderer, src);

        Shader.Unbind();
        dst.Unbind();
    }

    public override void DrawBuffer(IRenderer renderer, IFrameBuffer current, IFrameBuffer target)
    {
        sigma = 20 * Strength;
        kernelRadius = Blur.KernelSize(sigma);
        DrawColor = Colour4.White;

        buffer ??= renderer.CreateFrameBuffer();
        buffer.Size = current.Size;
        buffer2 ??= renderer.CreateFrameBuffer();
        buffer2.Size = current.Size;

        target.Unbind();

        drawBlurPass(renderer, current, buffer, Vector2.UnitX);
        drawBlurPass(renderer, buffer, buffer2, Vector2.UnitY);

        blurComposeShader.BindUniformBlock($"m_{nameof(BlurComposeParameters)}", composeParameterBuffer);
        target.Bind();

        blurComposeShader.Bind();
        renderer.BindTexture(buffer2.Texture, 1);
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct BlurComposeParameters
    {
        public UniformFloat Strength;
        private readonly UniformPadding12 pad1;
    }
}
