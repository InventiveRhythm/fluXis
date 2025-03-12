using fluXis.Screens.Gameplay.Overlay;
using NUnit.Framework;

namespace fluXis.Tests.Gameplay;

public partial class TestFullComboOverlay : FluXisTestScene
{
    private FullComboOverlay overlay;

    [SetUp]
    public void Setup() => Schedule(() => Child = overlay = new FullComboOverlay());

    [Test]
    public void TestFullCombo()
    {
        AddStep("show", () => overlay.Show(FullComboOverlay.FullComboType.FullCombo));
        hide();
    }

    [Test]
    public void TestAllFlawless()
    {
        AddStep("show", () => overlay.Show(FullComboOverlay.FullComboType.AllFlawless));
        hide();
    }

    private void hide()
    {
        AddWaitStep("wait", 8);
        AddStep("hide", () => overlay.Hide());
        AddWaitStep("wait", 8);
    }
}
