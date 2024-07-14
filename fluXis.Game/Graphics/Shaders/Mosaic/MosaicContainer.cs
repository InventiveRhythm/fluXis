using fluXis.Game.Map.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Mosaic;

public partial class MosaicContainer : ShaderContainer
{
    protected override string FragmentShader => "Mosaic";
    public override ShaderType Type => ShaderType.Mosaic;
    protected override DrawNode CreateShaderDrawNode() => new MosaicDrawNode(this, SharedData);
}
