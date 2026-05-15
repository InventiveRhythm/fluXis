using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Utils;

namespace fluXis.Graphics.Shaders.Steps;

public class FishEyeShaderStep : ShaderStep<FishEyeShaderStep.FishEyeParameters>
{
    protected override string FragmentShader => "FishEye";
    public override ShaderType Type => ShaderType.FishEye;

    public override bool ShouldRender => !Precision.AlmostEquals(Strength, 0);

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        Strength = Strength,
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct FishEyeParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Strength;
        private readonly UniformPadding4 pad1;
    }
}
