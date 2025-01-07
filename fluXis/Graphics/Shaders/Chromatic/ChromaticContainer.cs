using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.Chromatic;

public partial class ChromaticContainer : ShaderContainer
{
    protected override string FragmentShader => "ChromaticAberration";
    public override ShaderType Type => ShaderType.Chromatic;
    protected override DrawNode CreateShaderDrawNode() => new ChromaticContainerDrawNode(this, SharedData);
}
