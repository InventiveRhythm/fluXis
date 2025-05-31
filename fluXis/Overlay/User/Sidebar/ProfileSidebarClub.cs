using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Overlay.Club;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.User.Sidebar;

public partial class ProfileSidebarClub : ClickableContainer
{
    private APIClub club { get; }

    public ProfileSidebarClub(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader(true)]
    private void load([CanBeNull] ClubOverlay clubOverlay)
    {
        if (club is null)
        {
            Alpha = 0;
            return;
        }

        RelativeSizeAxes = Axes.X;
        Height = 80;
        CornerRadius = 12;
        Masking = true;
        Action = () => clubOverlay?.ShowClub(club.ID);
        Children = new Drawable[]
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
                Colour = FluXisColors.Background2.Opacity(.5f)
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
                        Size = new Vector2(80),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        CornerRadius = 12,
                        Masking = true,
                        OnComplete = d => d.FadeInFromZero(400),
                        LoadContent = () => new DrawableClubIcon(club)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
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
        };
    }
}
