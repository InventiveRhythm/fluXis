using osu.Framework.Graphics;

namespace fluXis.Game.Graphics.Shaders.Mosaic;

public partial class MosaicContainer : ShaderContainer
{
    protected override string FragmentShader => "Mosaic";
    protected override DrawNode CreateShaderDrawNode() => new MosaicDrawNode(this, SharedData);

    private float strength;

    /// <summary>
    /// The strength of the mosaic effect. From 0 to 1.
    /// <br/>
    /// 0 means its full resolution, 1 means its 1x1 pixel.
    /// </summary>
    public float Strength
    {
        get => strength;
        set
        {
            if (value == strength)
                return;

            strength = value;
            Invalidate(Invalidation.DrawNode);
        }
    }
}
