using System;
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
    protected string ComposeFragmentShader => "AdditiveComposeDithered";
    private IShader blurComposeShader;
    private IUniformBuffer<AdditiveComposeParameters> composeParameterBuffer;
    public override ShaderType Type => ShaderType.Bloom;

    public override int Passes => 1;

    private int kernelRadius;
    private float sigma;

    private const float max_sigma = 20f;

    private const float buffer_scale = 0.5f;

    public override void EnsureParameters(IRenderer renderer)
    {
        ParameterBuffer ??= renderer.CreateUniformBuffer<BlurParameters>();
        composeParameterBuffer ??= renderer.CreateUniformBuffer<AdditiveComposeParameters>();
    }

    public override void UpdateParameters(IFrameBuffer current)
    {
        composeParameterBuffer.Data = composeParameterBuffer.Data with
        {
            Strength = Strength,
        };
    }

    public override void LoadShader(ShaderManager shaders)
    {
        base.LoadShader(shaders);
        blurComposeShader = shaders.Load(VertexShader, ComposeFragmentShader);
    }

    private IFrameBuffer buffer;
    private IFrameBuffer buffer2;

    private void drawBlurPass(IRenderer renderer, IFrameBuffer src, IFrameBuffer dst, Vector2 blurDirection, Vector2 targetTexSize)
    {
        ParameterBuffer.Data = ParameterBuffer.Data with
        {
            Radius = kernelRadius,
            Sigma = sigma,
            TexSize = targetTexSize,
            Direction = blurDirection
        };

        Shader.BindUniformBlock($"m_{nameof(BlurParameters)}", ParameterBuffer);

        DrawScaledBuffer(renderer, src, dst, Shader, UsingVeldrid);
    }

    public override void DrawBuffer(IRenderer renderer, IFrameBuffer current, IFrameBuffer target)
    {
        float downsampleScale = Math.Max(0.5f, 1f - (Strength * 0.75f));
        sigma = max_sigma * Strength * downsampleScale;
        kernelRadius = Blur.KernelSize(sigma);
        DrawColor = Colour4.White;

        Vector2 bufferSize = new Vector2(
            (int)Math.Ceiling(current.Size.X * buffer_scale),
            (int)Math.Ceiling(current.Size.Y * buffer_scale)
        );

        EnsureBuffer(renderer, ref buffer, bufferSize);
        EnsureBuffer(renderer, ref buffer2, bufferSize);

        target.Unbind();

        drawBlurPass(renderer, current, buffer, Vector2.UnitX, bufferSize);
        drawBlurPass(renderer, buffer, buffer2, Vector2.UnitY, bufferSize);

        UpdateParameters(current);
        blurComposeShader.BindUniformBlock($"m_{nameof(AdditiveComposeParameters)}", composeParameterBuffer);

        //render composition
        target.Bind();
        blurComposeShader.Bind();

        renderer.BindTexture(buffer2.Texture, 1);

        DrawFrameBuffer(renderer, current);

        blurComposeShader.Unbind();

        renderer.BindTexture(renderer.WhitePixel, 1);
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
    public record struct AdditiveComposeParameters
    {
        public UniformFloat Strength;
        private readonly UniformPadding12 pad1;
    }
}
