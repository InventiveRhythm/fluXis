using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class ShatterShaderStep : ShaderStep<ShatterShaderStep.ShatterParameters>
{
    protected override string FragmentShader => "Shatter";
    public override ShaderType Type => ShaderType.Shatter;

    public override bool ShouldRender => Strength2 > 0;

    private float shaderTime { get; set; }

    public override void UpdateParameters(IFrameBuffer current)
    {
        shaderTime += (float)Time.Elapsed / 1000f * Strength3 * 3f;

        ParameterBuffer.Data = ParameterBuffer.Data with
        {
            TexSize = current.Size,
            CellSize = Strength * 10f,
            DistortionStrength = Strength2 * 0.2f,
            Time = shaderTime
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct ShatterParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat CellSize;
        public UniformFloat DistortionStrength;
        public UniformFloat Time;
        public UniformPadding12 pad;
    }
}
