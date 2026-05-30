using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class Glitch2ShaderStep : ShaderStep<Glitch2ShaderStep.Glitch2Parameters>
{
    protected override string FragmentShader => "Glitch2";
    public override ShaderType Type => ShaderType.Glitch2;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        StrengthX = Strength,
        StrengthY = Strength2,
        Time = (float)Time.Current / 1000f
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct Glitch2Parameters
    {
        public UniformVector2 TexSize;
        public UniformFloat StrengthX;
        public UniformFloat StrengthY;
        public UniformFloat Time;
        private readonly UniformPadding12 pad1;
    }
}
