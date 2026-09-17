using System.Text;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Tests.Graphics;

public partial class TestTextArea : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var sb = new StringBuilder();
        sb.AppendLine("This is a multiline textbox.");
        sb.AppendLine("It allows editing multiple lines of text.");
        sb.AppendLine();
        sb.AppendLine("It's also really complex to implement...");

        var area = new TextArea { RelativeSizeAxes = Axes.Both, Text = sb.ToString() };
        Add(area);

        AddStep("Add line", () => area.InsertString("Extra line!\n"));
    }
}
