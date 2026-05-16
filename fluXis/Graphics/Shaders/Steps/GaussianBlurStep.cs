using System;
using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Graphics.Shaders.Steps;

public class GaussianBlurStep : ShaderStep<GaussianBlurStep.BlurParameters>
{
    protected override string FragmentShader => "Blur";
    public override ShaderType Type => ShaderType.GaussianBlur;

    private int kernelRadius;
    private float sigma;
    private Vector2 direction;
    private Vector2 targetSize;

    private const float max_sigma = 32f;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = targetSize,
        Radius = kernelRadius,
        Sigma = sigma,
        Direction = direction
    };

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

        Vector2 downsampledSize = new Vector2(
            (int)Math.Ceiling(current.Size.X * downsampleScale),
            (int)Math.Ceiling(current.Size.Y * downsampleScale)
        );

        targetSize = downsampledSize;

        sigma = max_sigma * Strength * downsampleScale;
        kernelRadius = Blur.KernelSize(sigma);
        DrawColor = Colour4.White;

        EnsureBuffer(renderer, ref bufferX, downsampledSize);
        EnsureBuffer(renderer, ref bufferY, downsampledSize);

        target.Unbind();

        drawPass(renderer, current, bufferX, Vector2.UnitX, downsampledSize);
        drawPass(renderer, bufferX, bufferY, Vector2.UnitY, downsampledSize);

        DrawScaledBuffer(renderer, bufferY, target, null, UsingVeldrid);
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
