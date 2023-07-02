using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Scroll;
using fluXis.Game.Screens.Edit.Tabs.Timing;
using fluXis.Game.Screens.Edit.Tabs.Timing.List;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class TimingTab : EditorTab
{
    private readonly FluXisScrollContainer pointSettingsContainer;

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
                    new()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new TimingPointList(screen.MapInfo.TimingPoints, this),
                        new ScrollVelocityList(screen.MapInfo.ScrollVelocities, this),
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = FluXisColors.Background,
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding(20),
                                    Child = pointSettingsContainer = new FluXisScrollContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        ScrollbarAnchor = Anchor.TopRight
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    public void SetPointSettings(PointSettings pointSettings)
    {
        Schedule(() =>
        {
            pointSettings.Tab = this;
            pointSettingsContainer.Clear();
            pointSettingsContainer.Add(pointSettings);
        });
    }
}
