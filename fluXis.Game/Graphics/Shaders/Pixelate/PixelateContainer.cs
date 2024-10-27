using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Pixelate;

public partial class PixelateContainer : ShaderContainer
{
    protected override string FragmentShader => "Pixelate";
    public override ShaderType Type => ShaderType.Pixelate;
    protected override DrawNode CreateShaderDrawNode() => new PixelateContainerDrawNode(this, SharedData);
}
