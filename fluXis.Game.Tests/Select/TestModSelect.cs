using fluXis.Game.Screens.Select.Mods;

namespace fluXis.Game.Tests.Select;

public partial class TestModSelect : FluXisTestScene
{
    public TestModSelect()
    {
        var modSelector = new ModSelector();
        Add(modSelector);

        AddStep("Toggle Mod Selector", modSelector.IsOpen.Toggle);
    }
}
