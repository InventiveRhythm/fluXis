using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points;

public partial class SelectionInspector : FillFlowContainer
{
    private FillFlowContainer sectionFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(10);
        Padding = new MarginPadding(20);

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "Inspector",
                WebFontSize = 20
            },
            sectionFlow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10)
            }
        };
    }

    public void AddSection(string header, string value)
    {
        sectionFlow.Add(new Section(header, value));
    }

    public new void Clear() => sectionFlow.Clear();

    private partial class Section : FillFlowContainer
    {
        public Section(string header, string value)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = header,
                    WebFontSize = 14,
                    Alpha = .8f
                },
                new FluXisSpriteText
                {
                    Text = value,
                    WebFontSize = 18,
                    AllowMultiline = true
                }
            };
        }
    }
}

