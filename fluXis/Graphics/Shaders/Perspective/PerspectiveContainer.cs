using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.Perspective;

public partial class PerspectiveContainer : ShaderContainer
{
    protected override string FragmentShader => "Perspective";
    public override ShaderType Type => ShaderType.Perspective;
    protected override DrawNode CreateShaderDrawNode() => new PerspectiveContainerDrawNode(this, SharedData);
}