using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.FishEye;

public partial class FishEyeContainer : ShaderContainer
{
    protected override string FragmentShader => "FishEye";
    public override ShaderType Type => ShaderType.FishEye;
    protected override DrawNode CreateShaderDrawNode() => new FishEyeContainerDrawNode(this, SharedData);
}
