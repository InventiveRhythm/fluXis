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

        var set = maps.GetFromGuid("5955acf7-e356-459e-acf6-1456f869296f");
        var map = set.Maps.First(m => m.ID == Guid.Parse("91707210-223f-41b8-8ff5-3bc9613bbb2d"));
        GlobalClock.LoadMap(map);
        GlobalClock.Stop();

        var data = File.ReadAllText(@"W:\osu-lazer\exports\mylove\Raphlesia & BilliumMoto - My Love (Mao).osb");

        var parser = new OsuStoryboardParser();
        var storyboard = parser.Parse(data);

        var drawable = new DrawableStoryboard(storyboard, @"W:\osu-lazer\exports\mylove\");
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
