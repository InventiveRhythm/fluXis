using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Map.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay;

public partial class ShaderEventHandler : EventHandler<ShaderEvent>
{
    private ShaderStackContainer stack { get; }

    public ShaderEventHandler(List<ShaderEvent> events, ShaderStackContainer stack)
        : base(events)
    {
        this.stack = stack;
        Trigger = trigger;
    }

    private void trigger(ShaderEvent ev)
    {
        var shader = stack.GetShader(ev.Type);

        if (shader is null)
            throw new Exception($"Shader with type {ev.ShaderName} is not in stack!");

        shader.TransformTo(nameof(shader.Strength), ev.Parameters.Strength, ev.Duration);
    }
}
