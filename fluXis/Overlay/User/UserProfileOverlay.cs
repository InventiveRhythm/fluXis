using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Input;
using fluXis.Online;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables;
using fluXis.Overlay.User.Sections;
using fluXis.Overlay.User.Sidebar;
using fluXis.Overlay.User.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.User;

public partial class UserProfileOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private UserCache users { get; set; }

    private APIUser user;
    private Container content;
    private FluXisScrollContainer scroll;
    private FillFlowContainer flow;
    private OnlineErrorContainer error;
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
                    EdgeEffect = Styling.ShadowLargeNoOffset,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Theme.Background1
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
                        error = new OnlineErrorContainer
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
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
            error.Hide();
            loading.Show();
        });

        user = await users.UserAsync(id, true);

        if (user == null)
        {
            error.Text = "User not found.";
            error.Show();
            loading.Hide();
            return;
        }

        Schedule(() =>
        {
            loading.Hide();
            displayData(user);
        });
    }

    private void displayData(APIUser user)
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
                Padding = new MarginPadding { Horizontal = 12, Bottom = 12 },
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
                            new Dimension(GridSizeMode.Absolute, 320),
                            new Dimension(GridSizeMode.Absolute, 18),
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
                                        new ProfileSidebarClub(user.Club),
                                        new ProfileAboutMe(user.AboutMe),
                                        new ProfileSocials(user.Socials),
                                        new ProfileFollowerList(user.ID)
                                    }
                                },
                                Empty(),
                                new TabControl
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Tabs = new TabContainer[]
                                    {
                                        new ProfileScoresTab(user),
                                        new ProfileMapsTab(user)
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
        error.Hide();
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
