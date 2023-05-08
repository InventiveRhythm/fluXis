using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit.Timeline;

public partial class EditorBottomBar : Container
{
    public Editor Editor { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Origin = Anchor.BottomLeft;
        RelativeSizeAxes = Axes.X;
        Height = 60;
        Padding = new MarginPadding(5);

        Child = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 10,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new Dimension[]
                    {
                        new(GridSizeMode.Absolute, 200),
                        new(),
                        new(GridSizeMode.Absolute, 200),
                        new(GridSizeMode.Absolute, 100)
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new TimeInfo { BottomBar = this },
                            new EditorTimeline { BottomBar = this },
                            new PlaybackControl { BottomBar = this },
                            new EditorPlayTestButton
                            {
                                Action = () =>
                                {
                                    Editor.SortEverything();
                                    Editor.Push(new EditorPlaytestScreen(Editor.Map, Editor.MapInfo));
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
