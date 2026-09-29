using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class SliceShaderStep : ShaderStep<SliceShaderStep.SliceParameters>
{
    protected override string FragmentShader => "Slice";
    public override ShaderType Type => ShaderType.Slice;

    public override bool ShouldRender => Strength > 0;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        Strength = Strength,
        Angle = Strength2
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct SliceParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Strength;
        public UniformFloat Angle;
    }
}
