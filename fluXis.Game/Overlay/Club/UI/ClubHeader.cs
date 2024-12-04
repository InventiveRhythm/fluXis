using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API.Models.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.Club.UI;

public partial class ClubHeader : CompositeDrawable
{
    private APIClub club { get; }

    public ClubHeader(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 176;
        CornerRadius = 16;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new LoadWrapper<DrawableClubBanner>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => new DrawableClubBanner(club)
                {
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                OnComplete = background => background.FadeInFromZero(400)
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2.Opacity(.5f)
            },
            new GridContainer
            {
                Width = 960,
                Height = 128,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                ColumnDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 128),
                    new Dimension(GridSizeMode.Absolute, 20),
                    new Dimension()
                },
                Content = new[]
                {
                    new[]
                    {
                        new LoadWrapper<DrawableClubIcon>
                        {
                            Size = new Vector2(128),
                            CornerRadius = 12,
                            Masking = true,
                            EdgeEffect = FluXisStyles.ShadowMedium,
                            LoadContent = () => new DrawableClubIcon(club)
                            {
                                RelativeSizeAxes = Axes.Both,
                                FillMode = FillMode.Fill,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            OnComplete = cover => cover.FadeInFromZero(400)
                        },
                        Empty(),
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                new FluXisSpriteText
                                {
                                    Text = club.Name,
                                    WebFontSize = 30,
                                    Shadow = true,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft
                                },
                                new FluXisSpriteText
                                {
                                    Text = $"{club.Members!.Count} members",
                                    WebFontSize = 16,
                                    Shadow = true,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
