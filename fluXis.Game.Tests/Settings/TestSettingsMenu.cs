using fluXis.Game.Overlay.Settings;

namespace fluXis.Game.Tests.Settings;

public partial class TestSettingsMenu : FluXisTestScene
{
    public TestSettingsMenu()
    {
        var menu = new SettingsMenu();
        Add(menu);

        AddStep("Show", menu.Show);
        AddStep("Hide", menu.Hide);
    }
}
