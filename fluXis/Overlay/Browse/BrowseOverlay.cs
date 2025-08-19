using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Audio.Preview;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
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
using fluXis.Overlay.User.Header;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Utils;
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

    private FullInputBlockingContainer searchContainer;
    private HeaderButton scrollTopButton;
    private Container content;
    private FluXisScrollContainer scroll;
    private FillFlowContainer<MapCard> flow;
    private FillFlowContainer loading;

    private bool fetchingMore = true;
    private bool loadedAll;
    private string currentQuery = string.Empty;
    private bool firstOpen = true;
    private double previousScrollY = 0;

    private const float scroll_threshold = 50f;

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
                    EdgeEffect = Styling.ShadowLargeNoOffset,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Theme.Background1
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
                                    Padding = new MarginPadding { Top = 140, Bottom = 20, Horizontal = 20 },
                                    Spacing = new Vector2(20),
                                    Children = new Drawable[]
                                    {

                                        flow = new FillFlowContainer<MapCard>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Full,
                                            Spacing = new Vector2(20)
                                        },
                                        loading = new FillFlowContainer()
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(8),
                                            Padding = new MarginPadding { Vertical = 24 },
                                            Children = new Drawable[]
                                            {
                                                new LoadingIcon
                                                {
                                                    Anchor = Anchor.TopCentre,
                                                    Origin = Anchor.TopCentre,
                                                    Size = new Vector2(24)
                                                },
                                                new FluXisSpriteText
                                                {
                                                    Anchor = Anchor.TopCentre,
                                                    Origin = Anchor.TopCentre,
                                                    Text = "Loading...",
                                                    WebFontSize = 16,
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        searchContainer = new FullInputBlockingContainer
                        {
                            Height = 120,
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Padding = new MarginPadding { Top = 60, Horizontal = 20 },
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 135,
                                    Y = -60,
                                    Colour = Theme.Background1
                                },
                                new BrowserSearchBar
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    OnSearch = q =>
                                    {
                                        currentQuery = q;
                                        loadMapsets(reload: true);
                                    }
                                }
                            }
                        },
                        scrollTopButton = new HeaderButton
                        {
                            Margin = new MarginPadding(20),
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Icon = FontAwesome6.Solid.AngleUp,
                            UseAutoSize = false,
                            Size = new Vector2(64),
                            IconSize = new Vector2(24),
                            BackgroundColour = Theme.Background3,
                            Action = () => scroll.ScrollToStart(),
                            Alpha = 0
                        }
                    }
                }
            }
        };
    }

    private void loadMapsets(long offset = 0, bool reload = false)
    {
        fetchingMore = true;
        loading.FadeIn(200);

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
                    EdgeEffect = Styling.ShadowSmall,
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

            if (loadedAll)
                loading.FadeOut(200);
        };

        req.Failure += e =>
        {
            Logger.Log($"Failed to load mapsets: {e.Message}", LoggingTarget.Network);
            fetchingMore = false;
            loadedAll = true;
            loading.FadeOut(200);
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

        var end = scroll.ScrollableExtent - scroll.Current <= loading.DrawHeight;

        if (end && !fetchingMore && !loadedAll)
            loadMapsets(flow.Count);

        double currentScrollY = scroll.Current;
        double scrollDelta = currentScrollY - previousScrollY;

        if (currentScrollY > 1000)
        {
            if (!scrollTopButton.Enabled)
            {
                scrollTopButton.ScaleTo(1f, 400, Easing.OutQuart);
                scrollTopButton.FadeIn(400, Easing.OutQuart);
                scrollTopButton.Enabled = true;
            }
        }
        else
        {
            if (scrollTopButton.Enabled)
            {
                scrollTopButton.ScaleTo(0.5f, 400, Easing.OutQuart);
                scrollTopButton.FadeOut(400, Easing.OutQuart);
                scrollTopButton.Enabled = false;
            }
        }

        if (currentScrollY <= scroll_threshold)
            searchContainer.Y = 0;

        searchContainer.Y += -(float)scrollDelta;
        searchContainer.Y = Math.Clamp(searchContainer.Y, -100, 0);

        previousScrollY = currentScrollY;
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
