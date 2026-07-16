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

public class MotionBlurStep : ShaderStep<MotionBlurStep.BlurParameters>
{
    protected override string FragmentShader => "Blur";
    protected string DitherFragmentShader => "Dither";
    private IShader ditherShader;
    private IUniformBuffer<DitherParameters> ditherParameterBuffer;
    public override ShaderType Type => ShaderType.MotionBlur;

    public override bool ShouldRender => Strength > 0;

    private Vector2 blurDirection
    {
        get
        {
            float rad = MathHelper.DegreesToRadians(Strength2);
            return new Vector2(MathF.Cos(rad), MathF.Sin(rad));
        }
    }

    private float sigma;
    private int kernelRadius;
    private Vector2 targetSize;

    private const float max_sigma = 45f;

    private const float buffer_scale = 0.5f;

    private IFrameBuffer buffer;

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
            Direction = blurDirection,
            Radius = kernelRadius,
            Sigma = sigma
        };
        ditherParameterBuffer.Data = ditherParameterBuffer.Data with
        {
            Strength = Strength * 8,
        };
    }

    public override void LoadShader(ShaderManager shaders)
    {
        base.LoadShader(shaders);
        ditherShader = shaders.Load(VertexShader, DitherFragmentShader);
    }

    public override void DrawBuffer(IRenderer renderer, IFrameBuffer current, IFrameBuffer target)
    {
        float min_scale = 0.4f;
        float downsampleScale = Math.Max(min_scale, 1f - Strength);

        sigma = max_sigma * Strength * downsampleScale;
        kernelRadius = Blur.KernelSize(sigma);

        Vector2 bufferSize = new Vector2(
            (int)Math.Ceiling(current.Size.X * buffer_scale),
            (int)Math.Ceiling(current.Size.Y * buffer_scale)
        );

        targetSize = bufferSize;

        UpdateParameters(current);
        DrawColor = Colour4.White;

        EnsureBuffer(renderer, ref buffer, bufferSize);

        target.Unbind();

        // blur
        Shader.BindUniformBlock($"m_{nameof(BlurParameters)}", ParameterBuffer);
        DrawScaledBuffer(renderer, current, buffer, Shader, UsingVeldrid);

        // dither
        ditherShader.BindUniformBlock($"m_{nameof(DitherParameters)}", ditherParameterBuffer);
        DrawScaledBuffer(renderer, buffer, target, ditherShader, UsingVeldrid);
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
