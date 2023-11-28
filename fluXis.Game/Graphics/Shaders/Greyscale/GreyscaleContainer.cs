using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Greyscale;

public partial class GreyscaleContainer : ShaderContainer
{
    protected override string FragmentShader => "Greyscale";
    protected override DrawNode CreateShaderDrawNode() => new GreyscaleDrawNode(this, SharedData);
}
