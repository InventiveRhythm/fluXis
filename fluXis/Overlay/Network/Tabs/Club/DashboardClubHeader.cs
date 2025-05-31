using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Network.Tabs.Club;

public partial class DashboardClubHeader : CompositeDrawable
{
    private APIClub club { get; }

    public DashboardClubHeader(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 128;
        Masking = true;
        CornerRadius = 8;

        InternalChildren = new Drawable[]
        {
            new LoadWrapper<DrawableClubBanner>
            {
                RelativeSizeAxes = Axes.Both,
                OnComplete = d => d.FadeInFromZero(400),
                LoadContent = () => new DrawableClubBanner(club)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
                Alpha = .5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(12),
                Children = new Drawable[]
                {
                    new LoadWrapper<DrawableClubIcon>
                    {
                        Size = new Vector2(128),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        CornerRadius = 8,
                        Masking = true,
                        OnComplete = d => d.FadeInFromZero(400),
                        LoadContent = () => new DrawableClubIcon(club)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(4),
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.X,
                                Height = 24,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(6),
                                Children = new Drawable[]
                                {
                                    new ClubTag(club)
                                    {
                                        WebFontSize = 20,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Shadow = true
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = club.Name,
                                        WebFontSize = 24,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Shadow = true
                                    }
                                }
                            },
                            new ForcedHeightText
                            {
                                Text = $"{club.MemberCount} members",
                                WebFontSize = 16,
                                Height = 12,
                                Shadow = true
                            }
                        }
                    }
                }
            }
        };
    }
}
