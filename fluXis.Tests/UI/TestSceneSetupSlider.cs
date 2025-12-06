using fluXis.Screens.Edit.Tabs.Setup.Entries;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Tests.UI;

public partial class TestSceneSetupSlider : FluXisManualInputTestScene
{
    [Test]
    public void TestToggle()
    {
        var bind = new BindableNumber<float> { MaxValue = 4, MinValue = -4, Precision = 0.1f };
        SetupSlider<float> slider = null!;

        AddStep("create", () => Child = slider = new SetupSlider<float>("Slider text", bind)
        {
            RelativeSizeAxes = Axes.None,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Width = 480
        });
    }
}
