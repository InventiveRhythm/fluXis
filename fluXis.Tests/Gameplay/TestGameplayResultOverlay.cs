using fluXis.Screens.Gameplay.Overlay.Result;
using NUnit.Framework;
using osu.Framework.Graphics;

namespace fluXis.Tests.Gameplay;

public partial class TestGameplayResultOverlay : FluXisTestScene
{
    private Drawable overlay;

    [Test]
    public void TestFullCombo()
    {
        AddStep("add", () => Add(overlay = new FullComboResultOverlay()));
        AddStep("show", () => overlay.Show());
        hide();
    }

    [Test]
    public void TestAllFlawless()
    {
        AddStep("add", () => Add(overlay = new PureFlawlessResultOverlay()));
        AddStep("show", () => overlay.Show());
        hide();
    }

    private void hide()
    {
        AddWaitStep("wait", 8);
        AddStep("hide", () => overlay.Hide());
        AddWaitStep("wait", 8);
    }
}
