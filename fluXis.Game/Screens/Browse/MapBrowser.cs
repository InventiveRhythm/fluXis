using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Audio.Preview;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Requests.MapSets;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Auth;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Browse.Info;
using fluXis.Game.Screens.Browse.Search;
using fluXis.Shared.Components.Maps;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Browse;

public partial class MapBrowser : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float BackgroundDim => 0.75f;
    public override float BackgroundBlur => 0.5f;
    public override bool AllowMusicControl => false;
    public override UserActivity InitialActivity => new UserActivity.BrowsingMaps();

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private PreviewManager previews { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PanelContainer panels { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private MultifactorOverlay mfaOverlay { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    private readonly Bindable<APIMapSet> selectedSet = new();

    private bool fetchingMore = true;
    private bool loadedAll;
    private string currentQuery = string.Empty;

    private FluXisScrollContainer scroll;
    private FillFlowContainer<MapCard> maps { get; set; }
    private CornerButton backButton;
    private LoadingIcon loadingIcon;
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Spacing = new Vector2(10),
                Padding = new MarginPadding
                {
                    Horizontal = 120,
                    Vertical = 40
                },
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Icon = FontAwesome6.Solid.EarthAmericas,
                        Size = new Vector2(20)
                    },
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        FontSize = 24,
                        Text = "Online Map Browser"
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(100),
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new Dimension[]
                    {
                        new(),
                        new(GridSizeMode.Absolute, 40),
                        new(GridSizeMode.Absolute, 780)
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            new GridContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RowDimensions = new Dimension[]
                                {
                                    new(GridSizeMode.AutoSize),
                                    new(GridSizeMode.Absolute, 20),
                                    new()
                                },
                                Content = new[]
                                {
                                    new Drawable[] { new BrowserSearchBar { MapBrowser = this } },
                                    new[] { Empty() },
                                    new Drawable[]
                                    {
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Children = new Drawable[]
                                            {
                                                new FluXisContextMenuContainer
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Child = scroll = new FluXisScrollContainer
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        ScrollbarAnchor = Anchor.TopLeft,
                                                        Child = maps = new FillFlowContainer<MapCard>
                                                        {
                                                            RelativeSizeAxes = Axes.X,
                                                            AutoSizeAxes = Axes.Y,
                                                            Direction = FillDirection.Full,
                                                            Spacing = new Vector2(20)
                                                        }
                                                    }
                                                },
                                                loadingIcon = new LoadingIcon
                                                {
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Size = new Vector2(100)
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Empty(),
                            new BrowseInfo
                            {
                                BindableSet = selectedSet
                            }
                        }
                    }
                }
            },
            backButton = new CornerButton
            {
                Icon = FontAwesome6.Solid.ChevronLeft,
                ButtonText = LocalizationStrings.General.Back,
                Action = this.Exit
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                FontSize = 30
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (api.Status.Value != ConnectionStatus.Online)
        {
            text.Text = "You are not connected to the server!";
            text.FadeInFromZero(20);
            loadingIcon.Hide();
            return;
        }

        loadMapsets();
    }

    protected override void Update()
    {
        base.Update();

        if (scroll.IsScrolledToEnd() && !fetchingMore && !loadedAll)
            loadMapsets(maps.Count);
    }

    private void loadMapsets(long offset = 0, bool reload = false)
    {
        fetchingMore = true;
        loadingIcon.Show();

        var req = new MapSetsRequest(offset, currentQuery);
        req.Success += response =>
        {
            if (response.Data.Count == 0)
                loadedAll = true;

            if (reload)
                maps.Clear();

            foreach (var mapSet in response.Data)
            {
                maps.Add(new MapCard(mapSet)
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    EdgeEffect = FluXisStyles.ShadowSmall,
                    RequestDelete = confirmDeleteMapSet,
                    OnClickAction = set =>
                    {
                        previews.PlayPreview(set.ID);
                        selectedSet.Value = set;
                    }
                });
            }

            // needed, else it does 2 request when entering
            ScheduleAfterChildren(() => fetchingMore = false);
            loadingIcon.Hide();
        };

        req.Failure += e =>
        {
            Logger.Log($"Failed to load mapsets: {e.Message}", LoggingTarget.Network);
            fetchingMore = false;
            loadedAll = true;
            loadingIcon.Hide();
        };

        api.PerformRequestAsync(req);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.FadeInFromZero(FADE_DURATION);
            backButton.Show();

            clock.VolumeOut(FADE_DURATION);
            clock.Delay(FADE_DURATION).OnComplete(_ => clock.Stop());
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(FADE_DURATION);

        backButton.Hide();
        previews.StopPreview();

        clock.Start();
        clock.VolumeIn(FADE_DURATION);
        return false;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    public void Search(string query)
    {
        currentQuery = query;
        loadMapsets(reload: true);
    }

    private void confirmDeleteMapSet(long id)
    {
        if (panels == null || mfaOverlay == null)
            return;

        var panel = new ConfirmDeletionPanel(deleteMapSet, itemName: "MapSet");
        panels.Content = panel;

        void deleteMapSet()
        {
            var pnl = panels?.Content as Panel;
            pnl?.StartLoading();

            var req = new MapSetDeleteRequest(id);

            req.Failure += ex =>
            {
                pnl?.StopLoading();

                if (ex.Message == APIRequest.MULTIFACTOR_REQUIRED)
                {
                    mfaOverlay?.Show(deleteMapSet);
                    return;
                }

                notifications.SendError("An error occurred while deleting this mapset.", ex.Message);
            };

            req.Success += _ =>
            {
                maps.FirstOrDefault(x => x.MapSet.ID == id)?.Expire();
                pnl?.StopLoading();
            };

            api.PerformRequest(req);
        }
    }
}
