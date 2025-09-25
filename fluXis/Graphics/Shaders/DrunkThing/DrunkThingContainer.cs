using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.DrunkThing;

public partial class DrunkThingContainer : ShaderContainer
{
    protected override string FragmentShader => "DrunkThing";
    public override ShaderType Type => ShaderType.DrunkThing;
    protected override DrawNode CreateShaderDrawNode() => new DrunkThingContainerDrawNode(this, SharedData);
}