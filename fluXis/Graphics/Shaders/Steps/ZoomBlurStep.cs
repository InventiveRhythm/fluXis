using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Graphics.Shaders.Steps;

public class ZoomBlurStep : ShaderStep<ZoomBlurStep.BlurParameters>
{
    protected override string FragmentShader => "ZoomBlur";
    public override ShaderType Type => ShaderType.ZoomBlur;

    public override bool ShouldRender => !Precision.AlmostEquals(Strength, 0);

    public override void UpdateParameters(IFrameBuffer current)
    {
        float sigma = (Strength / 2f) + 0.5f;

        ParameterBuffer.Data = ParameterBuffer.Data with
        {
            TexSize = current.Size,
            Position = new Vector2((Strength2 + 1f) / 2f, (-Strength3 + 1f) / 2f),
            Sigma = sigma,
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct BlurParameters
    {
        public UniformVector2 TexSize;
        public UniformVector2 Position;
        public UniformFloat Sigma;
        public UniformPadding12 pad1;
    }
}
