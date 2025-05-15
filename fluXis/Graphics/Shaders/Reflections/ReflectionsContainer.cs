using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.Reflections;

public partial class ReflectionsContainer : ShaderContainer
{
    protected override string FragmentShader => "Reflections";
    public override ShaderType Type => ShaderType.Reflections;
    protected override DrawNode CreateShaderDrawNode() => new ReflectionsContainerDrawNode(this, SharedData);
}
