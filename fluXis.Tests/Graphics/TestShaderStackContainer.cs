using System;
using System.Collections.Generic;
using fluXis.Graphics.Background;
using fluXis.Graphics.Shaders;
using fluXis.Map;
using fluXis.Map.Structures.Events;
using fluXis.Mods;
using fluXis.Screens;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Capabilities;
using fluXis.Utils.Attributes;
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

        foreach (var shaderType in Enum.GetValues<ShaderType>())
        {
            var shd = ShaderStackContainer.CreateForType(shaderType);
            if (shd is null) continue;

            stack.AddShader(shd);

            var attrs = shd.Type.TryGetAllAttributes<ShaderType, ShaderStrengthAttribute>(out var a) ? a : [new ShaderStrengthAttribute()];

            foreach (var attr in attrs)
            {
                AddSliderStep($"{shaderType.ToString()} Strength {attr.Index}", attr.Min, attr.Max, 0, strength =>
                {
                    switch (attr.Index)
                    {
                        case 1:
                            shd.Strength = strength;
                            break;

                        case 2:
                            shd.Strength2 = strength;
                            break;

                        case 3:
                            shd.Strength3 = strength;
                            break;
                    }
                });
            }
        }

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
            var playable = map.GetPlayable(GameModes)!;
            var auto = GameModes.Find(playable.Mode)!.CreateAutoGenerator(playable);
            var replay = auto.Generate();

            screenStack.Push(new GameplayLoader(map, mods, () => new GameplayScreen(map, mods).RegisterCapability(new ReplayCapability(replay))));
        });
    }
}
