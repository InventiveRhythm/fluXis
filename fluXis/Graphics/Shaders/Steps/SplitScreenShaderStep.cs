using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;

namespace fluXis.Graphics.Shaders.Steps;

public class SplitScreenShaderStep : ShaderStep<SplitScreenShaderStep.SplitScreenParameters>
{
    protected override string FragmentShader => "SplitScreen";
    public override ShaderType Type => ShaderType.SplitScreen;

    public override bool ShouldRender => Strength > 0;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        SplitsInv = new Vector2(1.0f / Strength2, 1.0f / Strength3),
        Strength = Strength,
        SplitsX = (int)Strength2,
        SplitsY = (int)Strength3
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct SplitScreenParameters
    {
        public UniformVector2 TexSize;
        public UniformVector2 SplitsInv;
        public UniformFloat Strength;
        public UniformInt SplitsX;
        public UniformInt SplitsY;
        private readonly UniformPadding4 pad1;
    }
}
