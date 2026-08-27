using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Images;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Overlay.Navigator.Pages.UserV2;

#nullable enable

public partial class UserProfileClubTile : CompositeDrawable
{
    private readonly APIUser user;
    private readonly APIClub? club;

    public UserProfileClubTile(APIUser user, APIClub? club)
    {
        this.user = user;
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (club == null)
        {
            Alpha = 0;
            return;
        }

        RelativeSizeAxes = Axes.X;
        Height = 64;
        CornerRadius = 6;
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
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 6,
                Masking = true,
                LoadContent = () => new DrawableClubBanner(club).WíthRelativeSize(Axes.Both).WíthAnchor(Anchor.Centre),
                OnComplete = d => d.FadeInFromZero(Styling.TRANSITION_FADE),
                Alpha = .33f
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions =
                [
                    new Dimension(GridSizeMode.Absolute, 64),
                    new Dimension(GridSizeMode.Absolute, 16),
                    new Dimension()
                ],
                Content = new[]
                {
                    new[]
                    {
                        new LoadWrapper<DrawableClubIcon>
                        {
                            RelativeSizeAxes = Axes.Both,
                            CornerRadius = 6,
                            Masking = true,
                            LoadContent = () => new DrawableClubIcon(club).WíthRelativeSize(Axes.Both).WíthAnchor(Anchor.Centre),
                            OnComplete = d => d.FadeInFromZero(Styling.TRANSITION_FADE)
                        },
                        Empty(),
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Direction = FillDirection.Vertical,
                            Children =
                            [
                                new TruncatingText
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Text = club.Name,
                                    WebFontSize = 14
                                },
                                new TruncatingText
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Text = club.Owner?.ID == user.ID ? "Owner" : "Member",
                                    WebFontSize = 10,
                                    Alpha = .8f
                                }
                            ]
                        }
                    }
                }
            }
        ];
    }
}
