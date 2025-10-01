using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;
using osu.Framework.Testing.Input;
using osu.Framework.Utils;

namespace fluXis.Tests;

public partial class FluXisTestScene : TestScene
{
    protected DependencyContainer TestDependencies { get; private set; }
    protected GlobalClock GlobalClock => TestDependencies.Get<GlobalClock>();

    protected TestAPIClient TestAPI => TestDependencies.Get<IAPIClient>() as TestAPIClient;
    protected virtual bool UseTestAPI => false;

    protected void CreateClock()
    {
        var clock = new GlobalClock();
        TestDependencies.CacheAs(clock);
        TestDependencies.CacheAs<IBeatSyncProvider>(clock);
        TestDependencies.CacheAs<IAmplitudeProvider>(clock);
    }

    protected void CreateDummyBeatSync() => TestDependencies.CacheAs<IBeatSyncProvider>(new DummyBeatSyncProvider());
    protected void CreateDummyAmplitude() => TestDependencies.CacheAs<IAmplitudeProvider>(new DummyAmplitudeProvider());

    protected RealmMapSet CreateDummySet()
    {
        var map = RealmMap.CreateNew();
        var set = map.MapSet;

        set.ID = Guid.NewGuid();
        map.ID = Guid.NewGuid();

        set.Metadata.Title = set.ID.ToString();
        set.Metadata.Artist = "some artist";
        set.Metadata.Mapper = "some mapper";
        map.Difficulty = map.ID.ToString();
        map.Filters = new RealmMapFilters { NotesPerSecond = RNG.NextSingle(1, 30) };
        map.OnlineID = RNG.Next(1, 10000);
        map.Status = (MapStatus)RNG.Next((int)MapStatus.Local, (int)MapStatus.Pure + 1);

        return set;
    }

    protected virtual RealmMap GetTestMap(MapStore maps)
    {
        var set = maps.GetFromGuid(Program.TestSetID);

        if (set is null)
            return null;

        var id = Program.TestMapID;

        return string.IsNullOrEmpty(id)
            ? set.Maps.FirstOrDefault()
            : set.Maps.FirstOrDefault(m => m.ID == Guid.Parse(Program.TestMapID));
    }

    protected override ITestSceneTestRunner CreateRunner() => new FluXisTestSceneTestRunner();

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        TestDependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        if (UseTestAPI)
            TestDependencies.CacheAs<IAPIClient>(new TestAPIClient());

        return TestDependencies;
    }

    private partial class FluXisTestSceneTestRunner : FluXisGameBase, ITestSceneTestRunner
    {
        private TestSceneTestRunner.TestRunner runner;

        protected override void LoadAsyncComplete()
        {
            base.LoadAsyncComplete();
            Add(runner = new TestSceneTestRunner.TestRunner());
        }

        public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
    }

    private class DummyBeatSyncProvider : IBeatSyncProvider
    {
        public double StepTime => 200;
        public double BeatTime => StepTime * 4;
        public Action<int, bool> OnBeat { get; set; }
    }

    private class DummyAmplitudeProvider : IAmplitudeProvider
    {
        public float[] Amplitudes { get; } = new float[256];
    }
}

public partial class FluXisManualInputTestScene : FluXisTestScene
{
    protected override Container<Drawable> Content => content;
    private Container content { get; }

    protected virtual bool DisplayCursor => false;

    protected ManualInputManager Input { get; }

    protected FluXisManualInputTestScene()
    {
        var main = content = new Container { RelativeSizeAxes = Axes.Both };

        if (DisplayCursor)
        {
            var cursor = new GlobalCursorOverlay { RelativeSizeAxes = Axes.Both };
            cursor.Add(content = new GlobalTooltipContainer(cursor.Cursor) { RelativeSizeAxes = Axes.Both });
            main.Add(cursor);
        }

        base.Content.AddRange(new Drawable[]
        {
            Input = new ManualInputManager
            {
                UseParentInput = true,
                Child = main
            }
        });
    }
}
