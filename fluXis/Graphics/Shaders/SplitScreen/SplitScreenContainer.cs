using fluXis.Configuration;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Shaders.SplitScreen;

public partial class SplitScreenContainer : ShaderContainer
{
    protected override string FragmentShader => "SplitScreen";
    public override ShaderType Type => ShaderType.SplitScreen;
    protected override DrawNode CreateShaderDrawNode() => new SplitScreenContainerDrawNode(this, SharedData);
}
