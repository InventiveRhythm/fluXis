using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.Mosaic;

public partial class MosaicContainer : ShaderContainer
{
    protected override string FragmentShader => "Mosaic";
    public override ShaderType Type => ShaderType.Mosaic;
    protected override DrawNode CreateShaderDrawNode() => new MosaicDrawNode(this, SharedData);
}
