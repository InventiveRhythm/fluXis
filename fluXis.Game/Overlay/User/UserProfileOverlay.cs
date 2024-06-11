using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.User.Sections;
using fluXis.Game.Overlay.User.Sidebar;
using fluXis.Shared.Components.Clubs;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.User;

public partial class UserProfileOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private UserCache users { get; set; }

    private APIUser user;
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
                                Padding = new MarginPadding { Top = 70, Bottom = 20, Horizontal = 20 },
                                Spacing = new Vector2(20)
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

    public void ShowUser(long id)
    {
        var visible = State.Value == Visibility.Visible;
        Show();

        if (user?.ID != id)
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

        user = await users.UserAsync(id, true);
        if (user == null) return;

        var mapsReq = new UserMapsRequest(id);
        await fluxel.PerformRequestAsync(mapsReq);

        Schedule(() =>
        {
            loading.Hide();
            displayData(user, mapsReq.Response.Data);
        });
    }

    private void displayData(APIUser user, APIUserMaps maps)
    {
        flow.Show();
        flow.Children = new Drawable[]
        {
            new ProfileHeader(user),
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Horizontal = 10, Bottom = 20 },
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new ProfileStats(user.Statistics!),
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.Absolute, 300),
                            new Dimension(GridSizeMode.Absolute, 20),
                            new Dimension()
                        },
                        RowDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize)
                        },
                        Content = new[]
                        {
                            new[]
                            {
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(20),
                                    Children = new Drawable[]
                                    {
                                        new ProfileSidebarClub(user.Club ?? new APIClub())
                                        {
                                            Alpha = user.Club?.ID == 0 ? 0 : 1
                                        },
                                        new ProfileFollowerList(user.ID),
                                    }
                                },
                                Empty(),
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(20),
                                    Children = new Drawable[]
                                    {
                                        new ProfileMapsSection(maps)
                                    }
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
