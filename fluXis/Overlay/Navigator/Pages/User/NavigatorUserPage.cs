using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Users;
using fluXis.Overlay.Navigator.Pages.User.Sections;
using fluXis.Overlay.Navigator.Pages.User.Sidebar;
using fluXis.Overlay.Navigator.Pages.User.Tabs;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.User;

public partial class NavigatorUserPage : NavigatorPage<APIUser>
{
    public override string Path => $"/users/{UserID}";
    protected override float ContentWidth => 1280;

    public readonly long UserID;

    public NavigatorUserPage(long userID)
    {
        UserID = userID;
    }

    protected override APIUser PullData()
    {
        var req = new UserRequest(UserID);
        API.PerformRequest(req);
        req.ThrowIfFailed();
        return req.Response.Data;
    }

    protected override Drawable CreateContent(APIUser data) => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        AlwaysPresent = true,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(20),
        Children =
        [
            new ProfileHeader(data),
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Horizontal = 12, Bottom = 12 },
                Spacing = new Vector2(20),
                Children =
                [
                    new ProfileStats(data.Statistics!),
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        ColumnDimensions =
                        [
                            new Dimension(GridSizeMode.Absolute, 320),
                            new Dimension(GridSizeMode.Absolute, 18),
                            new Dimension()
                        ],
                        RowDimensions =
                        [
                            new Dimension(GridSizeMode.AutoSize)
                        ],
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
                                    Children =
                                    [
                                        new ProfileSidebarClub(data.Club),
                                        new ProfileAboutMe(data.AboutMe),
                                        new ProfileSocials(data.Socials),
                                        new ProfileFollowerList(data.ID)
                                    ]
                                },
                                Empty(),
                                new TabControl
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Tabs =
                                    [
                                        new ProfileScoresTab(data),
                                        new ProfileMapsTab(data)
                                    ]
                                }
                            }
                        }
                    }
                ]
            }
        ]
    };
}
