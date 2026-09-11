using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Shared.Points;

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

    public Section AddSection(LocalisableString header, LocalisableString value)
    {
        var s = new Section(header, value);
        sectionFlow.Add(s);
        return s;
    }

    public new void Clear() => sectionFlow.Clear();

    public partial class Section : FillFlowContainer
    {
        public string Value { set => text.Text = value; }

        private readonly FluXisSpriteText text;

        public Section(LocalisableString header, LocalisableString value)
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
                text = new FluXisSpriteText
                {
                    Text = value,
                    WebFontSize = 18,
                    AllowMultiline = true
                }
            };
        }
    }
}
