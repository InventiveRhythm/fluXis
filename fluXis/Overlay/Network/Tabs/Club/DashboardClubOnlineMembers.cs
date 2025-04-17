using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Network.Tabs.Club;

public partial class DashboardClubOnlineMembers : FillFlowContainer
{
    private APIClub club { get; }

    public DashboardClubOnlineMembers(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(12);
        Children = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                Height = 20,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(12),
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = "Online Members",
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 20
                    },
                    new FluXisSpriteText
                    {
                        Text = $"{club.Members.Count(x => x.IsOnline)}",
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 14,
                        Alpha = .6f
                    }
                }
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Full,
                Spacing = new Vector2(8),
                ChildrenEnumerable = club.Members.Where(x => x.IsOnline).Select(x => new DrawableUserCard(x)
                {
                    Width = 338
                })
            }
        };
    }
}
