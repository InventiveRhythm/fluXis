using fluXis.Game.Map.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Invert;

public partial class InvertContainer : ShaderContainer
{
    protected override string FragmentShader => "Invert";
    public override ShaderType Type => ShaderType.Invert;
    protected override DrawNode CreateShaderDrawNode() => new InvertContainerDrawNode(this, SharedData);
}
