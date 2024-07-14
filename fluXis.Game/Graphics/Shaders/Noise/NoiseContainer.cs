using fluXis.Game.Map.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Noise;

public partial class NoiseContainer : ShaderContainer
{
    protected override string FragmentShader => "Noise";
    public override ShaderType Type => ShaderType.Noise;
    protected override DrawNode CreateShaderDrawNode() => new NoiseContainerDrawNode(this, SharedData);
}
