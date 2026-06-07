using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class ReflectionsShaderStep : ShaderStep<ReflectionsShaderStep.ReflectionsParameters>
{
    protected override string FragmentShader => "Reflections";
    public override ShaderType Type => ShaderType.Reflections;

    public override bool ShouldRender => Strength > 0;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = new ReflectionsParameters
    {
        TexSize = current.Size,
        Strength = Strength,
        Scale = Strength2
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct ReflectionsParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Strength;
        public UniformFloat Scale;
    }
}
