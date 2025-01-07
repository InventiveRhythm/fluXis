using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.Retro;

public partial class RetroContainer : ShaderContainer
{
    protected override string FragmentShader => "Retro";
    public override ShaderType Type => ShaderType.Retro;
    protected override DrawNode CreateShaderDrawNode() => new RetroContainerDrawNode(this, SharedData);
}
