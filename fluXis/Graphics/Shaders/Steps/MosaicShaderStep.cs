using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class MosaicShaderStep : ShaderStep<MosaicShaderStep.MosaicParameters>
{
    protected override string FragmentShader => "Mosaic";
    public override ShaderType Type => ShaderType.Mosaic;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        Strength = Strength
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct MosaicParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Strength;
        private readonly UniformPadding4 pad1;
    }
}
