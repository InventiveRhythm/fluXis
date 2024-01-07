using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Mods;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Edit.BottomBar.Timeline;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit.BottomBar;

public partial class EditorBottomBar : Container
{
    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Origin = Anchor.BottomLeft;
        RelativeSizeAxes = Axes.X;
        Height = 60;

        Child = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    EdgeEffect = FluXisStyles.ShadowMediumNoOffset,
                    Masking = true,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                },
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new Dimension[]
                    {
                        new(GridSizeMode.Absolute, 200),
                        new(),
                        new(GridSizeMode.Absolute, 300),
                        new(GridSizeMode.Absolute, 270)
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new TimeInfo(),
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = FluXisColors.Background1
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding(20),
                                        Child = new EditorTimeline()
                                    }
                                }
                            },
                            new VariableControl(),
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new CornerButton
                                {
                                    ButtonText = "Test",
                                    Icon = FontAwesome.Solid.Play,
                                    ShowImmediately = true,
                                    ButtonColor = FluXisColors.Accent2,
                                    Corner = Corner.BottomRight,
                                    Action = () =>
                                    {
                                        if (values.Editor.Map == null)
                                        {
                                            notifications.SendError("Map is null!", "i dont know how this happened but it did");
                                            return;
                                        }

                                        if (values.Editor.MapInfo?.HitObjects == null || values.Editor.MapInfo.HitObjects.Count == 0)
                                        {
                                            notifications.SendError("This map has no hitobjects!");
                                            return;
                                        }

                                        if (values.Editor.MapInfo?.TimingPoints == null || values.Editor.MapInfo.TimingPoints.Count == 0)
                                        {
                                            notifications.SendError("This map has no timing points!");
                                            return;
                                        }

                                        values.Editor.SortEverything();

                                        clock.Stop();
                                        var startTime = clock.CurrentTime;

                                        var clone = values.Editor.MapInfo.Clone();
                                        clone.HitObjects = clone.HitObjects.Where(o => o.Time > startTime).ToList();

                                        values.Editor.Push(new GameplayLoader(values.Editor.Map, new List<IMod> { new NoFailMod() }, () => new EditorPlaytestScreen(values.Editor.Map, clone, values.MapEvents, startTime)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
