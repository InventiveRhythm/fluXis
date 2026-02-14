using System;
using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Graphics.Shaders.Bloom;

public class BloomShaderStep : ShaderStep<BloomShaderStep.BlurParameters>
{
    protected override string FragmentShader => "Blur";
    public override ShaderType Type => ShaderType.Bloom;

    public override bool AutoBindBuffer => false;

    private int kernelRadius;
    private float sigma;
    private float radians;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        Radius = kernelRadius,
        Sigma = sigma,
        TexSize = current.Size,
        Direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians))
    };

    public override void DrawBuffer(IRenderer renderer, IFrameBuffer current)
    {
        sigma = 20 * Strength;
        kernelRadius = Blur.KernelSize(sigma);
        DrawColor = Colour4.Blue.Opacity(Strength);

        renderer.PushScissorState(false);

        using (BindFrameBuffer(TargetBuffer))
            base.DrawBuffer(renderer, current);

        renderer.SetBlend(BlendingParameters.Additive);

        radians = float.DegreesToRadians(0);
        UpdateParameters(current);

        using (BindFrameBuffer(TargetBuffer))
            base.DrawBuffer(renderer, current);

        radians = float.DegreesToRadians(90);
        UpdateParameters(current);

        using (BindFrameBuffer(TargetBuffer))
            base.DrawBuffer(renderer, current);

        renderer.PopScissorState();
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
