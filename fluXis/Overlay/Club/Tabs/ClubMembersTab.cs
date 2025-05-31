using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Online.API.Models.Clubs;
using fluXis.Overlay.Club.Tabs.Members;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.Club.Tabs;

public partial class ClubMembersTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.UserGroup;
    public override string Title => "Members";

    private APIClub club { get; }

    public ClubMembersTab(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(12),
            ChildrenEnumerable = club.Members.Select(m => new ClubMemberEntry(club, m))
        };
    }
}
