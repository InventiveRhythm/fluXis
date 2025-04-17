using System.Linq;
using fluXis.Audio;
using fluXis.Audio.Preview;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Context;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Input;
using fluXis.Map.Drawables.Card;
using fluXis.Online.API;
using fluXis.Online.API.Requests.MapSets;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Auth;
using fluXis.Overlay.Notifications;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Overlay.Browse;

public partial class BrowseOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private PreviewManager previews { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PanelContainer panels { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private MultifactorOverlay mfaOverlay { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    private Container content;
    private FluXisScrollContainer scroll;
    private FillFlowContainer<MapCard> flow;
    private LoadingIcon loadingIcon;

    private bool fetchingMore = true;
    private bool loadedAll;
    private string currentQuery = string.Empty;
    private bool firstOpen = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Action = Hide,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = .5f
                    }
                }
            },
            new Container
            {
                Width = 1320,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = content = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Masking = true,
                    EdgeEffect = FluXisStyles.ShadowLargeNoOffset,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background1
                        },
                        new FluXisContextMenuContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = scroll = new FluXisScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ScrollbarVisible = false,
                                Child = new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Padding = new MarginPadding { Top = 70, Bottom = 20, Horizontal = 20 },
                                    Spacing = new Vector2(20),
                                    Children = new Drawable[]
                                    {
                                        new BrowserSearchBar
                                        {
                                            OnSearch = q =>
                                            {
                                                currentQuery = q;
                                                loadMapsets(reload: true);
                                            }
                                        },
                                        flow = new FillFlowContainer<MapCard>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Full,
                                            Spacing = new Vector2(20)
                                        }
                                    }
                                }
                            }
                        },
                        loadingIcon = new LoadingIcon
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(24)
                        }
                    }
                }
            }
        };
    }

    private void loadMapsets(long offset = 0, bool reload = false)
    {
        fetchingMore = true;
        loadingIcon.Show();

        if (reload)
            flow.Clear();

        var req = new MapSetsRequest(offset, 48, currentQuery);
        req.Success += response =>
        {
            loadedAll = response.Data.Count == 0;

            foreach (var mapSet in response.Data)
            {
                flow.Add(new MapCard(mapSet)
                {
                    CardWidth = 410,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    EdgeEffect = FluXisStyles.ShadowSmall,
                    RequestDelete = confirmDeleteMapSet,
                    OnClickAction = set =>
                    {
                        previews.PlayPreview(set.ID);
                        game?.PresentMapSet(set.ID);
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
                flow.FirstOrDefault(x => x.MapSet.ID == id)?.Expire();
                pnl?.StopLoading();
            };

            api.PerformRequest(req);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (scroll.IsScrolledToEnd() && !fetchingMore && !loadedAll)
            loadMapsets(flow.Count);
    }

    protected override void PopIn()
    {
        content.ResizeHeightTo(0).MoveToY(1)
               .ResizeHeightTo(1, 800, Easing.OutQuint)
               .MoveToY(0, 800, Easing.OutQuint);

        this.FadeIn(200);

        clock.VolumeOut(400).OnComplete(_ => clock.Stop());

        if (firstOpen)
        {
            loadMapsets();
            firstOpen = false;
        }
    }

    protected override void PopOut()
    {
        content.ResizeHeightTo(0, 800, Easing.OutQuint);
        this.FadeOut(200);

        previews.StopPreview();

        clock.Start();
        clock.VolumeIn(400);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back)
            return false;

        Hide();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
