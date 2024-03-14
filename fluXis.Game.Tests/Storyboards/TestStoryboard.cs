using System;
using System.IO;
using System.Linq;
using fluXis.Game.Map;
using fluXis.Game.Screens;
using fluXis.Game.Storyboards;
using fluXis.Game.Storyboards.Drawables;
using fluXis.Import.osu.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Tests.Storyboards;

public partial class TestStoryboard : FluXisTestScene
{
    [Resolved]
    private MapStore maps { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var screens = new FluXisScreenStack();
        TestDependencies.CacheAs(screens);

        Add(GlobalClock);

        var set = maps.GetFromGuid("e73344a8-9897-4149-b69a-71e03c5146af");
        var map = set.Maps.First(m => m.ID == Guid.Parse("adf5a2a9-df58-4558-92cf-c8fb0dd25cb0"));
        GlobalClock.LoadMap(map);
        GlobalClock.Stop();

        var data = File.ReadAllText(@"C:\Users\Flux\AppData\Roaming\osu\exports\neppa\Tephe - Neppa feat. Butter (Jiysea).osb");

        var parser = new OsuStoryboardParser();
        var storyboard = parser.Parse(data);

        var drawable = new DrawableStoryboard(storyboard, @"C:\Users\Flux\AppData\Roaming\osu\exports\neppa\");
        LoadComponent(drawable);

        Add(new Container
        {
            Size = new Vector2(854, 480),
            Clock = GlobalClock,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            Children = new Drawable[]
            {
                new DrawableStoryboardWrapper(GlobalClock, drawable, StoryboardLayer.Background),
                new DrawableStoryboardWrapper(GlobalClock, drawable, StoryboardLayer.Foreground),
                new DrawableStoryboardWrapper(GlobalClock, drawable, StoryboardLayer.Overlay)
            }
        });

        AddStep("Stop", GlobalClock.Stop);
        AddStep("Start", GlobalClock.Start);
    }
}
