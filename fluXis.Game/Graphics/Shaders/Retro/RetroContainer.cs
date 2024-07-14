using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Retro;

public partial class RetroContainer : ShaderContainer
{
    protected override string FragmentShader => "Retro";
    protected override DrawNode CreateShaderDrawNode() => new RetroContainerDrawNode(this, SharedData);

    /// <summary>
    /// The strength of the invert effect. From 0 to 1.
    /// </summary>
    public float Strength { get; set; }
}
