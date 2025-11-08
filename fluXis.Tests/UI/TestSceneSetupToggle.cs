using fluXis.Screens.Edit.Tabs.Setup.Entries;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osuTK.Input;

namespace fluXis.Tests.UI;

public partial class TestSceneSetupToggle : FluXisManualInputTestScene
{
    [Test]
    public void TestToggle()
    {
        var bind = new Bindable<bool>();

        SetupToggle toggle = null!;

        AddStep("create", () => Child = toggle = new SetupToggle("Toggle text", bind)
        {
            RelativeSizeAxes = Axes.None,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Width = 480
        });

        AddStep("move mouse", () => Input.MoveMouseTo(toggle));
        AddStep("click", () => Input.Click(MouseButton.Left));
        AddAssert("bind is true", () => bind.Value);
        AddStep("click", () => Input.Click(MouseButton.Left));
        AddAssert("bind is false", () => !bind.Value);
    }
}
