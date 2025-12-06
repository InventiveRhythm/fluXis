using System.IO;
using System.Linq;
using fluXis.Import.osu.Storyboards;
using fluXis.Map;
using fluXis.Screens;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Tests.Storyboards;

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

        var set = maps.MapSets.FirstOrDefault(s => s.Metadata.Title == "My Love" && s.Metadata.Artist == "Raphlesia & BilliumMoto");
        var map = set?.LowestDifficulty;

        if (map is not null)
        {
            GlobalClock.LoadMap(map);
            GlobalClock.Stop();
        }

        var data = File.ReadAllText(@"W:\osu-lazer\exports\mylove\Raphlesia & BilliumMoto - My Love (Mao).osb");

        var parser = new OsuStoryboardParser();
        var storyboard = parser.Parse(data);

        var drawable = new DrawableStoryboard(map?.GetMapInfo() ?? new MapInfo(), storyboard, @"W:\osu-lazer\exports\mylove\");
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
                new DrawableStoryboardLayer(GlobalClock, drawable, StoryboardLayer.Background),
                new DrawableStoryboardLayer(GlobalClock, drawable, StoryboardLayer.Foreground),
                new DrawableStoryboardLayer(GlobalClock, drawable, StoryboardLayer.Overlay)
            }
        });

        AddStep("Stop", GlobalClock.Stop);
        AddStep("Start", GlobalClock.Start);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.Left:
                GlobalClock.Seek(GlobalClock.CurrentTime - 2000);
                return true;

            case Key.Right:
                GlobalClock.Seek(GlobalClock.CurrentTime + 2000);
                return true;
        }

        return false;
    }
}
