using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.NeonThing;

public partial class NeonThingContainer : ShaderContainer
{
    protected override string FragmentShader => "NeonThing";
    public override ShaderType Type => ShaderType.NeonThing;
    protected override DrawNode CreateShaderDrawNode() => new NeonThingContainerDrawNode(this, SharedData);
}