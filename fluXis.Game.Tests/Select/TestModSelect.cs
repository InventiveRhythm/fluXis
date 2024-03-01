using fluXis.Game.Screens.Select.Mods;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Select;

public partial class TestModSelect : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var modSelector = new ModSelector();
        Add(modSelector);

        AddStep("Toggle Mod Selector", modSelector.IsOpen.Toggle);
    }
}
