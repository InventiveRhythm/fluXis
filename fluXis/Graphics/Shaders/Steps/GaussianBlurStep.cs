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

public class GaussianBlurStep : ShaderStep<GaussianBlurStep.BlurParameters>
{
    protected override string FragmentShader => "Blur";
    protected string DitherFragmentShader => "Dither";
    private IShader ditherShader;
    private IUniformBuffer<DitherParameters> ditherParameterBuffer;
    public override ShaderType Type => ShaderType.GaussianBlur;

    private int kernelRadius;
    private float sigma;
    private Vector2 direction;
    private Vector2 targetSize;

    private const float max_sigma = 32f;

    private const float buffer_scale = 0.5f;

    public override void EnsureParameters(IRenderer renderer)
    {
        ParameterBuffer ??= renderer.CreateUniformBuffer<BlurParameters>();
        ditherParameterBuffer ??= renderer.CreateUniformBuffer<DitherParameters>();
    }

    public override void UpdateParameters(IFrameBuffer current)
    {
        ParameterBuffer.Data = ParameterBuffer.Data with
        {
            TexSize = targetSize,
            Radius = kernelRadius,
            Sigma = sigma,
            Direction = direction
        };
        ditherParameterBuffer.Data = ditherParameterBuffer.Data with
        {
            Strength = Strength * 6,
        };
    }

    public override void LoadShader(ShaderManager shaders)
    {
        base.LoadShader(shaders);
        ditherShader = shaders.Load(VertexShader, DitherFragmentShader);
    }

    private IFrameBuffer bufferX;
    private IFrameBuffer bufferY;

    private void drawPass(IRenderer renderer, IFrameBuffer src, IFrameBuffer dst, Vector2 dir, Vector2 targetTexSize)
    {
        direction = dir;
        targetSize = targetTexSize;
        UpdateParameters(src);

        Shader.BindUniformBlock($"m_{nameof(BlurParameters)}", ParameterBuffer);

        DrawScaledBuffer(renderer, src, dst, Shader, UsingVeldrid);
    }

    public override void DrawBuffer(IRenderer renderer, IFrameBuffer current, IFrameBuffer target)
    {
        float min_scale = 0.5f;
        float downsampleScale = Math.Max(min_scale, 1f - Strength);

        sigma = max_sigma * Strength * downsampleScale;
        kernelRadius = Blur.KernelSize(sigma);
        DrawColor = Colour4.White;

        Vector2 bufferSize = new Vector2(
            (int)Math.Ceiling(current.Size.X * buffer_scale),
            (int)Math.Ceiling(current.Size.Y * buffer_scale)
        );

        targetSize = bufferSize;

        EnsureBuffer(renderer, ref bufferX, bufferSize);
        EnsureBuffer(renderer, ref bufferY, bufferSize);

        target.Unbind();

        drawPass(renderer, current, bufferX, Vector2.UnitX, bufferSize);
        drawPass(renderer, bufferX, bufferY, Vector2.UnitY, bufferSize);

        ditherShader.BindUniformBlock($"m_{nameof(DitherParameters)}", ditherParameterBuffer);

        DrawScaledBuffer(renderer, bufferY, target, ditherShader, UsingVeldrid);
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
    private record struct DitherParameters
    {
        public UniformFloat Strength;
        private readonly UniformPadding12 pad1;
    }
}
