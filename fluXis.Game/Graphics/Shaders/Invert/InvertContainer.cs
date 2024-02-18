using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Invert;

public partial class InvertContainer : ShaderContainer
{
    protected override string FragmentShader => "Invert";
    protected override DrawNode CreateShaderDrawNode() => new InvertContainerDrawNode(this, SharedData);

    /// <summary>
    /// The strength of the invert effect. From 0 to 1.
    /// </summary>
    public float Strength { get; set; } = 1f;
}
