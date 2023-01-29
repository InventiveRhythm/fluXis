using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs;

public class TimingTab : EditorTab
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
                new TimingCategoryList<TimingPointInfo>("Timing Points", screen.Map.TimingPoints, Colour4.FromHex("#2a2a30")),
                new TimingCategoryList<ScrollVelocityInfo>("Scroll Velocities", screen.Map.ScrollVelocities, Colour4.FromHex("#222228")),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 1f / 3f,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.FromHex("#1a1a20"),
                    }
                }
            }
        });
    }
}
