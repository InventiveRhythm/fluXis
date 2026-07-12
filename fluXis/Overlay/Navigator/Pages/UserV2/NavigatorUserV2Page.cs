using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Drawables.Images;
using fluXis.Overlay.Navigator.Pages.User.Tabs;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.UserV2;

public partial class NavigatorUserV2Page : NavigatorPage<APIUser>
{
    public override string Path => $"/users/{UserID}";
    protected override float ContentWidth => 1344;
    public override bool AllowScrolling => false;

    public readonly long UserID;

    public NavigatorUserV2Page(long userID)
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

    protected override Drawable CreateContent(APIUser data) => new Container
    {
        RelativeSizeAxes = Axes.Both,
        Padding = new MarginPadding { Vertical = 64 },
        Child = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            ColumnDimensions =
            [
                new Dimension(GridSizeMode.Absolute, 480),
                new Dimension(GridSizeMode.Absolute, 24),
                new Dimension()
            ],
            Content = new[]
            {
                new[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children =
                        [
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Children =
                                [
                                    new UserProfileHeader(data)
                                ]
                            }
                        ]
                    },
                    Empty(),
                    new FluXisScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ScrollbarVisible = false,
                        Masking = false,
                        Child = new TabControl
                        {
                            Tabs =
                            [
                                new ProfileScoresTab(data),
                                new ProfileMapsTab(data) { CardWidth = 412 }
                            ]
                        }
                    }
                }
            }
        }
    };

    protected override Drawable CreateBackground(APIUser data) => new Container
    {
        RelativeSizeAxes = Axes.Both,
        Children =
        [
            new LoadWrapper<DrawableBanner>
            {
                RelativeSizeAxes = Axes.X,
                Height = 511,
                Alpha = 1 / 3f,
                Masking = true,
                LoadContent = () => new DrawableBanner(data)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                OnComplete = d => d.FadeInFromZero(Styling.TRANSITION_FADE)
            },
            new SectionedGradient
            {
                Width = 240,
                RelativeSizeAxes = Axes.Y,
                SplitPoint = 0.1f,
                Colour = Theme.Background1
            },
            new SectionedGradient
            {
                Width = 240,
                RelativeSizeAxes = Axes.Y,
                SplitPoint = 0.1f,
                Colour = Theme.Background1,
                Anchor = Anchor.TopRight,
                Scale = new Vector2(-1, 1)
            }
        ]
    };
}
