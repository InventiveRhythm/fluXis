using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class TimingTab : EditorTab
{
    public TimingTab(Editor screen)
        : base(screen)
    {
        AddInternal(new FillFlowContainer
        {
            Direction = FillDirection.Horizontal,
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                new TimingCategoryList<TimingPointInfo>("Timing Points", screen.MapInfo.TimingPoints, FluXisColors.Surface),
                new TimingCategoryList<ScrollVelocityInfo>("Scroll Velocities", screen.MapInfo.ScrollVelocities, FluXisColors.Background2),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 1f / 3f,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background,
                    }
                }
            }
        });
    }
}
