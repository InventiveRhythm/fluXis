using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Mods;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Edit.BottomBar.Timeline;
using fluXis.Game.Screens.Edit.Playtest;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit.BottomBar;

public partial class EditorBottomBar : Container
{
    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private Editor editor { get; set; }

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
                        new(GridSizeMode.Absolute, 96),
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
                                        Padding = new MarginPadding { Horizontal = 20 },
                                        Child = new EditorTimeline()
                                    }
                                }
                            },
                            new SnapControl(),
                            new VariableControl(),
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new CornerButton
                                {
                                    ButtonText = "Test",
                                    Icon = FontAwesome6.Solid.Play,
                                    ShowImmediately = true,
                                    ButtonColor = FluXisColors.Primary,
                                    Corner = Corner.BottomRight,
                                    Action = () =>
                                    {
                                        if (map.MapInfo == null)
                                        {
                                            notifications.SendError("Map is null!", "i dont know how this happened but it did");
                                            return;
                                        }

                                        if (map.MapInfo?.HitObjects == null || map.MapInfo.HitObjects.Count == 0)
                                        {
                                            notifications.SendError("This map has no hitobjects!");
                                            return;
                                        }

                                        if (map.MapInfo?.TimingPoints == null || map.MapInfo.TimingPoints.Count == 0)
                                        {
                                            notifications.SendError("This map has no timing points!");
                                            return;
                                        }

                                        map.Sort();

                                        clock.Stop();
                                        var startTime = clock.CurrentTime;

                                        var clone = map.MapInfo.DeepClone();
                                        clone.HitObjects = clone.HitObjects.Where(o => o.Time > startTime).ToList();

                                        var shouldAutoPlay = GetContainingInputManager().CurrentState.Keyboard.ControlPressed;

                                        var mods = new List<IMod>();

                                        if (shouldAutoPlay)
                                            mods.Add(new AutoPlayMod());
                                        else
                                            mods.Add(new NoFailMod());

                                        editor.Push(new GameplayLoader(map.RealmMap, mods, () =>
                                        {
                                            if (shouldAutoPlay)
                                                return new EditorAutoPlaytestScreen(map.RealmMap, clone, startTime);

                                            return new EditorPlaytestScreen(map.RealmMap, clone, startTime);
                                        }));
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
