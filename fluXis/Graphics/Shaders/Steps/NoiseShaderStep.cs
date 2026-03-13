using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class NoiseShaderStep : ShaderStep<NoiseShaderStep.NoiseParameters>
{
    protected override string FragmentShader => "Noise";
    public override ShaderType Type => ShaderType.Noise;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = new NoiseParameters
    {
        TexSize = current.Size,
        Strength = Strength,
        Time = (float)(Time.Current / 1000f)
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct NoiseParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Strength;
        public UniformFloat Time;
    }
}
