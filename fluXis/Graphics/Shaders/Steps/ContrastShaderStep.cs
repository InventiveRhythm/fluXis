using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class ContrastShaderStep : ShaderStep<ContrastShaderStep.ContrastParameters>
{
    protected override string FragmentShader => "Contrast";
    public override ShaderType Type => ShaderType.Contrast;

    public override bool ShouldRender => Strength > 0;

    public override void UpdateParameters(IFrameBuffer current)
    {
        ParameterBuffer.Data = ParameterBuffer.Data with
        {
            TexSize = current.Size,
            Strength = Strength
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct ContrastParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Strength;
        public UniformPadding4 pad;
    }
}
