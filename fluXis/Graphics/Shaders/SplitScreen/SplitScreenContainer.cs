using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.SplitScreen;

public partial class SplitScreenContainer : ShaderContainer
{
    protected override string FragmentShader => "SplitScreen";
    public override ShaderType Type => ShaderType.SplitScreen;
    protected override DrawNode CreateShaderDrawNode() => new SplitScreenContainerDrawNode(this, SharedData);
}
