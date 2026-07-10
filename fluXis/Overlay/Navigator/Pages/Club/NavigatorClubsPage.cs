using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.API.Requests.Clubs;
using fluXis.Online.Drawables.Images;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.Club;

public partial class NavigatorClubsPage : NavigatorPage<List<APIClub>>
{
    public override string Path => "/clubs";
    protected override float ContentWidth => 1280;

    private const float spacing = 16;
    private const float columns = 3;

    protected override List<APIClub> PullData()
    {
        var req = new ClubsRequest();
        API.PerformRequest(req);
        req.ThrowIfFailed();
        return req.Response.Data;
    }

    protected override Drawable CreateContent(List<APIClub> data)
    {
        var width = (ContentWidth - spacing * (columns - 1)) / columns;

        return new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(spacing),
            Direction = FillDirection.Full,
            ChildrenEnumerable = data.Select(x => new ClubTile(x)
            {
                Width = width,
                Action = () => Navigator.PushClub(x.ID)
            })
        };
    }

    private partial class ClubTile : ClickableContainer
    {
        public ClubTile(APIClub club)
        {
            Height = 224;
            CornerRadius = 20;
            Masking = true;

            InternalChildren =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2
                },
                new LoadWrapper<DrawableClubBanner>
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 144,
                    CornerRadius = CornerRadius,
                    Masking = true,
                    LoadContent = () => new DrawableClubBanner(club)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                    OnComplete = b => b.FadeInFromZero(Styling.TRANSITION_FADE)
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 80,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Padding = new MarginPadding(16),
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions =
                        [
                            new Dimension(GridSizeMode.Absolute, 48),
                            new Dimension(GridSizeMode.Absolute, 8),
                            new Dimension()
                        ],
                        Content = new[]
                        {
                            new[]
                            {
                                new LoadWrapper<DrawableClubIcon>
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    CornerRadius = 8,
                                    Masking = true,
                                    LoadContent = () => new DrawableClubIcon(club)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre
                                    },
                                    OnComplete = i => i.FadeInFromZero(Styling.TRANSITION_FADE)
                                },
                                Empty(),
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Direction = FillDirection.Vertical,
                                    Masking = true,
                                    Children =
                                    [
                                        new FluXisSpriteText
                                        {
                                            Text = club.Name,
                                            WebFontSize = 16
                                        },
                                        new FluXisSpriteText
                                        {
                                            Text = $"{club.MemberCount} member{(club.MemberCount > 1 ? "s" : "")}",
                                            WebFontSize = 12
                                        }
                                    ]
                                }
                            }
                        }
                    }
                },
                new FillFlowContainer
                {
                    Spacing = new Vector2(8)
                }
            ];
        }
    }
}
