using osu.Framework.Allocation;
using osu.Framework.Testing;

namespace fluXis.Game.Tests;

public partial class FluXisTestScene : TestScene
{
    protected DependencyContainer TestDependencies { get; private set; }

    protected override ITestSceneTestRunner CreateRunner() => new FluXisTestSceneTestRunner();

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => TestDependencies = new DependencyContainer(base.CreateChildDependencies(parent));

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
}
