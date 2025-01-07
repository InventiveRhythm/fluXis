using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.Invert;

public partial class InvertContainer : ShaderContainer
{
    protected override string FragmentShader => "Invert";
    public override ShaderType Type => ShaderType.Invert;
    protected override DrawNode CreateShaderDrawNode() => new InvertContainerDrawNode(this, SharedData);
}
