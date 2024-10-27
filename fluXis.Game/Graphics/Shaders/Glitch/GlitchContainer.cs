using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Glitch;

public partial class GlitchContainer : ShaderContainer
{
    protected override string FragmentShader => "Glitch";
    public override ShaderType Type => ShaderType.Glitch;
    protected override DrawNode CreateShaderDrawNode() => new GlitchContainerDrawNode(this, SharedData);
}
