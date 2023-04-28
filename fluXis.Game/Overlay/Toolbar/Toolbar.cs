using fluXis.Game.Graphics;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class Toolbar : Container
{
    [Resolved]
    private SettingsMenu settings { get; set; }

    public FluXisScreenStack ScreenStack { get; set; }
    public MenuScreen MenuScreen { get; set; }

    public BindableBool ShowToolbar = new();

    private FillFlowContainer leftFlow;
    private FillFlowContainer rightFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Padding = new MarginPadding { Vertical = 5, Horizontal = 10 };

        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Shear = new Vector2(0.1f, 0),
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 10 },
                Children = new Drawable[]
                {
                    leftFlow = new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            new ToolbarButton
                            {
                                Name = "Home",
                                Icon = FontAwesome.Solid.Home,
                                Action = () => MenuScreen?.MakeCurrent()
                            },
                            new ToolbarSeperator(),
                            new ToolbarButton
                            {
                                Name = "Maps",
                                Icon = FontAwesome.Solid.Map,
                                Action = () =>
                                {
                                    if (ScreenStack?.CurrentScreen is not SelectScreen)
                                    {
                                        MenuScreen?.MakeCurrent();
                                        ScreenStack?.Push(new SelectScreen());
                                    }
                                }
                            },
                            new ToolbarButton
                            {
                                Name = "Settings",
                                Icon = FontAwesome.Solid.Cog,
                                Action = () => settings.ToggleVisibility()
                            },
                            new ToolbarSeperator(),
                            new ToolbarButton
                            {
                                Name = "Ranking",
                                Icon = FontAwesome.Solid.ChartBar
                            },
                            new ToolbarButton
                            {
                                Name = "Download Maps",
                                Icon = FontAwesome.Solid.Download
                            },
                            new ToolbarButton
                            {
                                Name = "Dashboard",
                                Icon = FontAwesome.Solid.ChartLine
                            },
                            new ToolbarButton
                            {
                                Name = "Wiki",
                                Icon = FontAwesome.Solid.Book
                            },
                            new ToolbarButton
                            {
                                Name = "Music Player",
                                Icon = FontAwesome.Solid.Music
                            },
                        }
                    },
                    rightFlow = new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Children = new Drawable[]
                        {
                            new ToolbarProfile(),
                            new ToolbarClock()
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        ShowToolbar.BindValueChanged(OnShowToolbarChanged, true);
    }

    private void OnShowToolbarChanged(ValueChangedEvent<bool> e)
    {
        if (e.OldValue == e.NewValue) return;

        this.MoveToY(e.NewValue ? 0 : -Height, 500, Easing.OutQuint);
    }
}
