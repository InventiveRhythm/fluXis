using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class GlitchShaderStep : ShaderStep<GlitchShaderStep.GlitchParameters>
{
    protected override string FragmentShader => "Glitch";
    public override ShaderType Type => ShaderType.Glitch;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        StrengthX = Strength / 10f,
        StrengthY = Strength2 / 10f,
        BlockSize = Strength3,
        Time = (float)Time.Current % 10000f
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct GlitchParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat StrengthX;
        public UniformFloat StrengthY;
        public UniformFloat BlockSize;
        public UniformFloat Time;
        private readonly UniformPadding8 pad1;
    }
}
