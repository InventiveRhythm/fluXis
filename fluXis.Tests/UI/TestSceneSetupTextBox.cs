using fluXis.Screens.Edit.Tabs.Setup.Entries;
using NUnit.Framework;
using osu.Framework.Graphics;

namespace fluXis.Tests.UI;

public partial class TestSceneSetupTextBox : FluXisTestScene
{
    [Test]
    public void TestTextBox()
    {
        AddStep("create", () =>
        {
            Child = new SetupTextBox("Title")
            {
                RelativeSizeAxes = Axes.None,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 480
            };
        });
    }
}
