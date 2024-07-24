using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Tabs;
using fluXis.Game.Input;
using fluXis.Game.Online.API.Requests.Clubs;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Club.Sidebar;
using fluXis.Game.Overlay.Club.Tabs;
using fluXis.Game.Overlay.Club.UI;
using fluXis.Shared.Components.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Club;

public partial class ClubOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private IAPIClient api { get; set; }

    private APIClub club;
    private Container content;
    private FluXisScrollContainer scroll;
    private FillFlowContainer flow;
    private LoadingIcon loading;

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
                        scroll = new FluXisScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarVisible = false,
                            Child = flow = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                AlwaysPresent = true,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding { Top = 70, Bottom = 36, Horizontal = 20 },
                                Spacing = new Vector2(32)
                            }
                        },
                        loading = new LoadingIcon
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(50),
                            Alpha = 0
                        }
                    }
                }
            }
        };
    }

    public void ShowClub(long id)
    {
        var visible = State.Value == Visibility.Visible;
        Show();

        if (club?.ID != id)
            fetch(id, visible);
    }

    private async void fetch(long id, bool wasVisible)
    {
        if (wasVisible)
        {
            Schedule(() => flow.FadeOut(200).OnComplete(_ => fetch(id, false)));
            return;
        }

        Schedule(() =>
        {
            flow.Clear();
            loading.Show();
        });

        var request = new ClubRequest(id);
        await api.PerformRequestAsync(request);

        // we might want to show a message if the request fails
        if (!request.IsSuccessful)
            return;

        club = request.Response!.Data;
        Schedule(() =>
        {
            displayData(club);
            loading.Hide();
        });
    }

    private void displayData(APIClub club)
    {
        flow.Show();
        flow.Children = new Drawable[]
        {
            new ClubHeader(club),
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Horizontal = 16 },
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.Absolute, 32),
                        new Dimension(GridSizeMode.Absolute, 300),
                    },
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            new TabControl
                            {
                                RelativeSizeAxes = Axes.X,
                                Tabs = new TabContainer[]
                                {
                                    new ClubMembersTab(club),
                                    new ClubScoresTab(),
                                }
                            },
                            Empty(),
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(28),
                                Children = new Drawable[]
                                {
                                    new ClubSidebarStats(club),
                                    new ClubSidebarActivity(club),
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void PopIn()
    {
        content.ResizeHeightTo(0).MoveToY(1)
               .ResizeHeightTo(1, 800, Easing.OutQuint)
               .MoveToY(0, 800, Easing.OutQuint);

        scroll.ScrollTo(0, false);
        scroll.FadeOut().Delay(400).FadeIn(200);
        this.FadeIn(200);
    }

    protected override void PopOut()
    {
        content.ResizeHeightTo(0, 800, Easing.OutQuint);
        this.FadeOut(200);
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

