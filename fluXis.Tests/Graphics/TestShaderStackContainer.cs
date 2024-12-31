using System.Collections.Generic;
using fluXis.Graphics.Background;
using fluXis.Graphics.Shaders;
using fluXis.Graphics.Shaders.Chromatic;
using fluXis.Graphics.Shaders.Greyscale;
using fluXis.Graphics.Shaders.Invert;
using fluXis.Graphics.Shaders.Mosaic;
using fluXis.Graphics.Shaders.Noise;
using fluXis.Graphics.Shaders.Retro;
using fluXis.Graphics.Shaders.Vignette;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Replays;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Tests.Graphics;

public partial class TestShaderStackContainer : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore maps)
    {
        var map = GetTestMap(maps);

        if (map is null)
            return;

        CreateClock();

        var backgrounds = new GlobalBackground();
        TestDependencies.Cache(backgrounds);

        var screenStack = new FluXisScreenStack { RelativeSizeAxes = Axes.Both };
        TestDependencies.Cache(screenStack);

        var stack = new ShaderStackContainer { RelativeSizeAxes = Axes.Both };

        var chroma = new ChromaticContainer { RelativeSizeAxes = Axes.Both };
        stack.AddShader(chroma);
        AddSliderStep("Chroma Strength", 0, 20, 0, strength => chroma.Strength = strength);

        var grey = new GreyscaleContainer { RelativeSizeAxes = Axes.Both };
        stack.AddShader(grey);
        AddSliderStep("Greyscale Strength", 0, 1f, 0f, strength => grey.Strength = strength);

        var invert = new InvertContainer { RelativeSizeAxes = Axes.Both, Strength = 0 };
        stack.AddShader(invert);
        AddSliderStep("Invert Strength", 0, 1f, 0f, strength => invert.Strength = strength);

        var mosaic = new MosaicContainer { RelativeSizeAxes = Axes.Both, Strength = 0 };
        stack.AddShader(mosaic);
        AddSliderStep("Mosaic Strength", 0, 1f, 0f, strength => mosaic.Strength = strength);

        var noise = new NoiseContainer { RelativeSizeAxes = Axes.Both, Strength = 0 };
        stack.AddShader(noise);
        AddSliderStep("Noise Strength", 0, 1f, 0f, strength => noise.Strength = strength);

        var vignette = new VignetteContainer { RelativeSizeAxes = Axes.Both };
        stack.AddShader(vignette);
        AddSliderStep("Vignette Strength", 0, 1f, 0f, strength => vignette.Strength = strength);

        var retro = new RetroContainer { RelativeSizeAxes = Axes.Both };
        stack.AddShader(retro);
        AddSliderStep("Retro Strength", 0, 1f, 0f, strength => retro.Strength = strength);

        stack.AddContent(new Drawable[]
        {
            GlobalClock,
            backgrounds,
            screenStack
        });

        Add(stack);

        AddStep("Push Loader", () =>
        {
            if (screenStack.CurrentScreen is not null) return;

            var mods = new List<IMod> { new AutoPlayMod() };
            var replay = new AutoGenerator(map.GetMapInfo(), map.KeyCount);

            screenStack.Push(new GameplayLoader(map, mods, () => new ReplayGameplayScreen(map, mods, replay.Generate())));
        });
    }
}
