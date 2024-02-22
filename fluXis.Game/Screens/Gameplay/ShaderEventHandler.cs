using System.Collections.Generic;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Graphics.Shaders.Bloom;
using fluXis.Game.Graphics.Shaders.Chromatic;
using fluXis.Game.Graphics.Shaders.Greyscale;
using fluXis.Game.Graphics.Shaders.Invert;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Events.Shader;
using osu.Framework.Graphics;
using osu.Framework.Logging;

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
        switch (ev.ShaderName)
        {
            case "Invert":
                invert(ev);
                break;

            case "Greyscale":
                greyscale(ev);
                break;

            case "Chromatic":
                chromatic(ev);
                break;

            case "Bloom":
                bloom(ev);
                break;
        }
    }

    private void invert(ShaderEvent ev)
    {
        var data = ev.ParamsAs<ShaderStrengthParams>();
        var invert = stack.GetShader<InvertContainer>();

        if (invert == null)
            throw new System.Exception("Invert shader not found");

        invert.TransformTo(nameof(invert.Strength), data.Strength, ev.Duration);
    }

    private void greyscale(ShaderEvent ev)
    {
        Logger.Log($"Greyscale at {Clock.CurrentTime}", LoggingTarget.Runtime, LogLevel.Debug);
        var data = ev.ParamsAs<ShaderStrengthParams>();
        var greyscale = stack.GetShader<GreyscaleContainer>();

        if (greyscale == null)
            throw new System.Exception("Greyscale shader not found");

        greyscale.TransformTo(nameof(greyscale.Strength), data.Strength, ev.Duration);
    }

    private void chromatic(ShaderEvent ev)
    {
        Logger.Log($"Greyscale at {Clock.CurrentTime}", LoggingTarget.Runtime, LogLevel.Debug);
        var data = ev.ParamsAs<ShaderStrengthParams>();
        var chromatic = stack.GetShader<ChromaticContainer>();

        if (chromatic == null)
            throw new System.Exception("Chromatic shader not found");

        chromatic.TransformTo(nameof(chromatic.Strength), data.Strength, ev.Duration);
    }

    private void bloom(ShaderEvent ev)
    {
        Logger.Log($"Bloom at {Clock.CurrentTime}", LoggingTarget.Runtime, LogLevel.Debug);
        var data = ev.ParamsAs<ShaderStrengthParams>();
        var bloom = stack.GetShader<BloomContainer>();

        if (bloom == null)
            throw new System.Exception("Bloom shader not found");

        bloom.TransformTo(nameof(bloom.Strength), data.Strength, ev.Duration);
    }
}
