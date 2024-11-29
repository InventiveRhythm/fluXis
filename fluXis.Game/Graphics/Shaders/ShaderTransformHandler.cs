using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Graphics.Shaders;

public partial class ShaderTransformHandler : CompositeComponent
{
    public override bool RemoveCompletedTransforms => false;

    public ShaderType Type => shader.Type;

    private ShaderContainer shader { get; }

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

    public ShaderTransformHandler(ShaderContainer shader)
    {
        this.shader = shader;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
    }
}
