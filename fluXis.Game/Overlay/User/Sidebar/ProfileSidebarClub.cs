using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Shared.Components.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.User.Sidebar;

public partial class ProfileSidebarClub : FillFlowContainer
{
    private APIClub club { get; }

    public ProfileSidebarClub(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 10);

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Horizontal = 10 },
                Child = new FluXisSpriteText
                {
                    Text = "Club",
                    WebFontSize = 24
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 80,
                CornerRadius = 10,
                Masking = true,
                Children = new Drawable[]
                {
                    new LoadWrapper<DrawableClubBanner>
                    {
                        RelativeSizeAxes = Axes.Both,
                        LoadContent = () => new DrawableClubBanner(club)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        OnComplete = d => d.FadeInFromZero(400)
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2.Opacity(.5f)
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10),
                        Children = new Drawable[]
                        {
                            new LoadWrapper<DrawableClubIcon>
                            {
                                Size = new Vector2(80),
                                CornerRadius = 10,
                                Masking = true,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                LoadContent = () => new DrawableClubIcon(club)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                },
                                OnComplete = d => d.FadeInFromZero(400)
                            },
                            new FluXisSpriteText
                            {
                                Text = club.Name,
                                WebFontSize = 20,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    }
                }
            }
        };
    }
}
