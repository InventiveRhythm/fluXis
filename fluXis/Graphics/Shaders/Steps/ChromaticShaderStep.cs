using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class ChromaticShaderStep : ShaderStep<ChromaticShaderStep.ChromaticParameters>
{
    protected override string FragmentShader => "ChromaticAberration";
    public override ShaderType Type => ShaderType.Chromatic;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        Radius = Strength
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct ChromaticParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Radius;
        private readonly UniformPadding4 pad1;
    }
}
