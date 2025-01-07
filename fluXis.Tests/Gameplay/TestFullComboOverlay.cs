using fluXis.Screens.Gameplay.Overlay;
using osu.Framework.Allocation;

namespace fluXis.Tests.Gameplay;

public partial class TestFullComboOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var overlay = new FullComboOverlay();
        Add(overlay);

        AddLabel("Full Combo");
        AddStep("Show", () => overlay.Show(FullComboOverlay.FullComboType.FullCombo));
        AddWaitStep("Wait", 8);
        AddStep("Hide", () => overlay.Hide());

        AddLabel("All Flawless");
        AddStep("Show", () => overlay.Show(FullComboOverlay.FullComboType.AllFlawless));
        AddWaitStep("Wait", 8);
        AddStep("Hide", () => overlay.Hide());
    }
}
