using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class WarpShaderStep : ShaderStep<WarpShaderStep.WarpParameters>
{
    protected override string FragmentShader => "Warp";
    public override ShaderType Type => ShaderType.Warp;

    public override bool ShouldRender => Strength3 > 0;

    private float shaderTime { get; set; }

    public override void UpdateParameters(IFrameBuffer current)
    {
        shaderTime += (float)Time.Elapsed / 1000f * Strength * 3f;

        ParameterBuffer.Data = ParameterBuffer.Data with
        {
            TexSize = current.Size,
            Scale = Strength2 + 4f,
            Iterations = Strength3,
            Time = shaderTime
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct WarpParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Scale;
        public UniformFloat Iterations;
        public UniformFloat Time;
        public UniformPadding12 pad;
    }
}
