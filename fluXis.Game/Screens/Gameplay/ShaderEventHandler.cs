using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay;

public partial class ShaderEventHandler : EventHandler<ShaderEvent>
{
    private ShaderStackContainer stack { get; }
    private ShaderType[] types { get; }

    public ShaderEventHandler(List<ShaderEvent> events, ShaderStackContainer stack)
        : base(events)
    {
        types = events.Select(x => x.Type).Distinct().ToArray();
        this.stack = stack;
        Trigger = trigger;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        foreach (var shaderType in types)
        {
            var shader = stack.GetShader(shaderType);

            if (shader is null)
                throw new Exception($"Shader with type {shaderType} is not in stack!");

            AddInternal(new TransformHandler(shader));
        }
    }

    private void trigger(ShaderEvent ev)
    {
        var handler = InternalChildren.OfType<TransformHandler>().FirstOrDefault(x => x.Type == ev.Type);

        if (handler is null)
            throw new Exception($"Handler with type {ev.ShaderName} is not in scene tree!");

        handler.StrengthTo(ev.Parameters.Strength, ev.Duration, ev.Easing);
    }

    // the shader stack is outside the gameplay clock.
    // we use this to keep up with rate changes and seeking
    private partial class TransformHandler : Drawable
    {
        private ShaderContainer container { get; }
        public ShaderType Type { get; }

        private float strength
        {
            get => container.Strength;
            set => container.Strength = value;
        }

        public TransformHandler(ShaderContainer container)
        {
            this.container = container;
            Type = container.Type;
        }

        public void StrengthTo(float str, double dur = 0, Easing ease = Easing.None)
            => this.TransformTo(nameof(strength), str, dur, ease);
    }
}
