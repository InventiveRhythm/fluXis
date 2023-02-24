using fluXis.Game.Graphics;
using fluXis.Game.Screens.Edit.Tabs.Timing.List;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class TimingTab : EditorTab
{
    public TimingTab(Editor screen)
        : base(screen)
    {
        AddInternal(new Container
        {
            CornerRadius = 10,
            Masking = true,
            RelativeSizeAxes = Axes.Both,
            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new Dimension[]
                {
                    new(),
                    new(),
                    new(),
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new TimingPointList(screen.MapInfo.TimingPoints),
                        new ScrollVelocityList(screen.MapInfo.ScrollVelocities),
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background,
                            }
                        }
                    }
                }
            }
        });
    }
}
