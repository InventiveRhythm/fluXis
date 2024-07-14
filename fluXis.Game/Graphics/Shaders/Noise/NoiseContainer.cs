using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Noise;

public partial class NoiseContainer : ShaderContainer
{
    protected override string FragmentShader => "Noise";
    protected override DrawNode CreateShaderDrawNode() => new NoiseContainerDrawNode(this, SharedData);

    /// <summary>
    /// The strength of the invert effect. From 0 to 1.
    /// </summary>
    public float Strength { get; set; }
}
