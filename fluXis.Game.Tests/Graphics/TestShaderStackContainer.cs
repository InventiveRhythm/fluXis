using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Graphics.Shaders.Chromatic;
using fluXis.Game.Graphics.Shaders.Greyscale;
using fluXis.Game.Graphics.Shaders.Invert;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Replays;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Gameplay.Replay;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Tests.Graphics;

public partial class TestShaderStackContainer : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore maps)
    {
        const string set_id = "9896365c-5541-4612-9f39-5a44aa1012ed";
        const string map_id = "3b55b380-e533-4eea-bf16-4b98d9776583";

        var map = maps.GetFromGuid(set_id)?.Maps.FirstOrDefault(m => m.ID == Guid.Parse(map_id));
        if (map is null) return;

        var clock = new GlobalClock();
        TestDependencies.Cache(clock);

        var backgrounds = new GlobalBackground();
        TestDependencies.Cache(backgrounds);

        var screenStack = new FluXisScreenStack { RelativeSizeAxes = Axes.Both };

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

        stack.AddContent(new Drawable[]
        {
            clock,
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
