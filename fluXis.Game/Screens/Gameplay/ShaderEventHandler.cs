using System.Collections.Generic;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Graphics.Shaders.Bloom;
using fluXis.Game.Graphics.Shaders.Chromatic;
using fluXis.Game.Graphics.Shaders.Greyscale;
using fluXis.Game.Graphics.Shaders.Invert;
using fluXis.Game.Graphics.Shaders.Mosaic;
using fluXis.Game.Graphics.Shaders.Noise;
using fluXis.Game.Graphics.Shaders.Retro;
using fluXis.Game.Graphics.Shaders.Vignette;
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

            case "Mosaic":
                mosaic(ev);
                break;

            case "Noise":
                noise(ev);
                break;

            case "Vignette":
                vignette(ev);
                break;

            case "Retro":
                retro(ev);
                break;
        }
    }

    private void invert(ShaderEvent ev)
    {
        var data = ev.Parameters;
        var invert = stack.GetShader<InvertContainer>();

        if (invert == null)
            throw new System.Exception("Invert shader not found");

        invert.TransformTo(nameof(invert.Strength), data.Strength, ev.Duration);
    }

    private void greyscale(ShaderEvent ev)
    {
        var data = ev.Parameters;
        var greyscale = stack.GetShader<GreyscaleContainer>();

        if (greyscale == null)
            throw new System.Exception("Greyscale shader not found");

        greyscale.TransformTo(nameof(greyscale.Strength), data.Strength, ev.Duration);
    }

    private void chromatic(ShaderEvent ev)
    {
        var data = ev.Parameters;
        var chromatic = stack.GetShader<ChromaticContainer>();

        if (chromatic == null)
            throw new System.Exception("Chromatic shader not found");

        chromatic.TransformTo(nameof(chromatic.Strength), data.Strength, ev.Duration);
    }

    private void bloom(ShaderEvent ev)
    {
        var data = ev.Parameters;
        var bloom = stack.GetShader<BloomContainer>();

        if (bloom == null)
            throw new System.Exception("Bloom shader not found");

        bloom.TransformTo(nameof(bloom.Strength), data.Strength, ev.Duration);
    }

    private void mosaic(ShaderEvent ev)
    {
        var data = ev.Parameters;
        var mosaic = stack.GetShader<MosaicContainer>();

        if (mosaic == null)
            throw new System.Exception("Mosaic shader not found");

        mosaic.TransformTo(nameof(mosaic.Strength), data.Strength, ev.Duration);
    }

    private void noise(ShaderEvent ev)
    {
        var data = ev.Parameters;
        var noise = stack.GetShader<NoiseContainer>();

        if (noise == null)
            throw new System.Exception("Noise shader not found");

        noise.TransformTo(nameof(noise.Strength), data.Strength, ev.Duration);
    }

    private void vignette(ShaderEvent ev)
    {
        var data = ev.Parameters;
        var vignette = stack.GetShader<VignetteContainer>();

        if (vignette == null)
            throw new System.Exception("Vignette shader not found");

        vignette.TransformTo(nameof(vignette.Strength), data.Strength, ev.Duration);
    }

    private void retro(ShaderEvent ev)
    {
        var data = ev.Parameters;
        var retro = stack.GetShader<RetroContainer>();

        if (retro == null)
            throw new System.Exception("Retro shader not found");

        retro.TransformTo(nameof(retro.Strength), data.Strength, ev.Duration);
    }
}
