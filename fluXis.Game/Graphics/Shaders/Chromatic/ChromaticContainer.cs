using fluXis.Game.Map.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Chromatic;

public partial class ChromaticContainer : ShaderContainer
{
    protected override string FragmentShader => "ChromaticAberration";
    public override ShaderType Type => ShaderType.Chromatic;
    protected override DrawNode CreateShaderDrawNode() => new ChromaticContainerDrawNode(this, SharedData);
}
