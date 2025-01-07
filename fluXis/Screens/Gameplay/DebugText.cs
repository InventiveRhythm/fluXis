using System.Collections.Generic;
using System.Text;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay;

public partial class DebugText : CompositeDrawable
{
    private readonly FluXisTextFlow flow;
    private readonly List<string> lines = new();

    public DebugText()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(64);
        InternalChild = flow = new FluXisTextFlow { RelativeSizeAxes = Axes.X };
    }

    public void AddLine(string line)
    {
        lines.Add(line);
    }

    protected override void Update()
    {
        base.Update();

        var sb = new StringBuilder();
        lines.ForEach(l => sb.AppendLine(l));
        flow.Text = sb.ToString();
        lines.Clear();
    }
}
