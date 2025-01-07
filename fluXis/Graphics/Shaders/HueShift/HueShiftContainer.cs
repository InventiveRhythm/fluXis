using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.HueShift;

public partial class HueShiftContainer : ShaderContainer
{
    protected override string FragmentShader => "HueShift";
    public override ShaderType Type => ShaderType.HueShift;
    protected override DrawNode CreateShaderDrawNode() => new HueShiftContainerDrawNode(this, SharedData);
}
