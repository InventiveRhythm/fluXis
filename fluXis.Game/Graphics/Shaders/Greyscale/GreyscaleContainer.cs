using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Greyscale;

public partial class GreyscaleContainer : ShaderContainer
{
    protected override string FragmentShader => "Greyscale";
    protected override DrawNode CreateShaderDrawNode() => new GreyscaleDrawNode(this, SharedData);

    /// <summary>
    /// The strength of the greyscale effect. From 0 to 1.
    /// </summary>
    public float Strength { get; set; }
}
