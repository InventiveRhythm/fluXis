using fluXis.Configuration;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Shaders.Reflections;

public partial class ReflectionsContainer : ShaderContainer
{
    protected override string FragmentShader => "Reflections";
    public override ShaderType Type => ShaderType.Reflections;
    protected override DrawNode CreateShaderDrawNode() => new ReflectionsContainerDrawNode(this, SharedData);
}
