using osu.Framework.Allocation;

namespace fluXis.Game.Tests;

public partial class TestFluXisGame : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AddGame(new FluXisGame());
    }
}
