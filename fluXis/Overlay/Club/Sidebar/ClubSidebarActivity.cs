using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Club.Sidebar;

public partial class ClubSidebarActivity : FillFlowContainer
{
    private APIClub club { get; }

    public ClubSidebarActivity(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(8);

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "Activity",
                WebFontSize = 24
            },
            new FluXisSpriteText
            {
                Text = "Nothing here yet...",
                WebFontSize = 12,
                Alpha = .8f
            }
        };
    }
}
