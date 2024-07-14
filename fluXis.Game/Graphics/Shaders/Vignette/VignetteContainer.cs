using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Vignette;

public partial class VignetteContainer : ShaderContainer
{
    protected override string FragmentShader => "Vignette";
    protected override DrawNode CreateShaderDrawNode() => new VignetteContainerDrawNode(this, SharedData);

    /// <summary>
    /// The strength of the invert effect. From 0 to 1.
    /// </summary>
    public float Strength { get; set; }
}
