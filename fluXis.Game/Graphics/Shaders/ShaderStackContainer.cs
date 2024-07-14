using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Game.Graphics.Shaders;

public partial class ShaderStackContainer : CompositeDrawable
{
    private readonly List<ShaderContainer> shaders = new();

    public ShaderStackContainer()
    {
        RelativeSizeAxes = Axes.Both;
    }

    public void AddShader(ShaderContainer shader)
    {
        Logger.Log($"Adding shader {shader.GetType().Name} to stack", LoggingTarget.Runtime, LogLevel.Debug);
        Logger.Log($"{shaders.Count}", LoggingTarget.Runtime, LogLevel.Debug);

        if (shaders.Count == 0)
            InternalChild = shader;
        else
            shaders.Last().Add(shader);

        shaders.Add(shader);
    }

    public ShaderStackContainer AddContent(Drawable[] content)
    {
        if (shaders.Count == 0)
            InternalChildren = content;
        else
            shaders.Last().AddRange(content);

        return this;
    }

    public T GetShader<T>() where T : ShaderContainer
        => shaders.FirstOrDefault(s => s.GetType() == typeof(T)) as T;

    public ShaderContainer GetShader(ShaderType type)
        => shaders.FirstOrDefault(s => s.Type == type);
}
