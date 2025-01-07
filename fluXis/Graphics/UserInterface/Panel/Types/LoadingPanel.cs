using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Graphics.UserInterface.Panel.Types;

public partial class LoadingPanel : Panel
{
    public string Text { get; set; }

    public string SubText
    {
        get => subText;
        set
        {
            subText = value;

            if (subTextFlow == null) return;

            if (!ThreadSafety.IsUpdateThread)
                Schedule(() => subTextFlow.Text = value);
            else
                subTextFlow.Text = value;
        }
    }

    private string subText;
    private FluXisTextFlow subTextFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 800;
        Height = 200;

        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Spacing = new Vector2(0, 10),
            Children = new Drawable[]
            {
                new FluXisTextFlow
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Text = Text,
                    FontSize = 30,
                    TextAnchor = Anchor.TopCentre,
                    Shadow = true,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                },
                subTextFlow = new FluXisTextFlow
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Text = SubText,
                    FontSize = 20,
                    Colour = FluXisColors.Text2,
                    TextAnchor = Anchor.TopCentre,
                    Shadow = true,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                }
            }
        };
    }
}
