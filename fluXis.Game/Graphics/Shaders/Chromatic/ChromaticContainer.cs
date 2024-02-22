using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Chromatic;

public partial class ChromaticContainer : ShaderContainer
{
    protected override string FragmentShader => "ChromaticAberration";
    protected override DrawNode CreateShaderDrawNode() => new ChromaticContainerDrawNode(this, SharedData);

    /// <summary>
    /// The strength of the chromatic aberration effect. In pixels.
    /// </summary>
    public float Strength { get; set; }
}
