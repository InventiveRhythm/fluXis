using System;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Testing;

namespace fluXis.Game.Tests;

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
        public Action<int> OnBeat { get; set; }
    }

    private class DummyAmplitudeProvider : IAmplitudeProvider
    {
        public float[] Amplitudes { get; } = new float[256];
    }
}
