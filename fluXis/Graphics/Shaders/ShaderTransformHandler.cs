using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Shaders;

public partial class ShaderTransformHandler : CompositeComponent, IHasStrength
{
    public override bool RemoveCompletedTransforms => false;

    private IHasStrength shader { get; }

    public float Strength
    {
        get => shader.Strength;
        set => shader.Strength = value;
    }

    public float Strength2
    {
        get => shader.Strength2;
        set => shader.Strength2 = value;
    }

    public float Strength3
    {
        get => shader.Strength3;
        set => shader.Strength3 = value;
    }

    public ShaderTransformHandler(IHasStrength shader)
    {
        this.shader = shader;
        Name = (shader as ShaderStep)?.Type.ToString() ?? "";
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
    }
}
