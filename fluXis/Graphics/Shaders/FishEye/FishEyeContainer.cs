using fluXis.Configuration;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Shaders.FishEye;

public partial class FishEyeContainer : ShaderContainer
{
    protected override string FragmentShader => "FishEye";
    public override ShaderType Type => ShaderType.FishEye;
    protected override DrawNode CreateShaderDrawNode() => new FishEyeContainerDrawNode(this, SharedData);
}
