using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.Noise;

public partial class NoiseContainer : ShaderContainer
{
    protected override string FragmentShader => "Noise";
    public override ShaderType Type => ShaderType.Noise;
    protected override DrawNode CreateShaderDrawNode() => new NoiseContainerDrawNode(this, SharedData);
}
