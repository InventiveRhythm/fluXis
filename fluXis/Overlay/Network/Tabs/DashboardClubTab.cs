using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Clubs;
using fluXis.Online.Drawables;
using fluXis.Online.Drawables.Users;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Network.Tabs.Club;
using fluXis.Overlay.Network.Tabs.Shared;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardClubTab : DashboardTab
{
    public override LocalisableString Title => LocalizationStrings.Dashboard.Club;
    public override IconUsage Icon => FontAwesome6.Solid.CircleNodes;
    public override DashboardTabType Type => DashboardTabType.Club;

    [Resolved]
    private IAPIClient api { get; set; }

    private FluXisScrollContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        Header.Child = new DashboardRefreshButton(refresh);

        Content.Children = new Drawable[]
        {
            content = new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = false
            }
        };
    }

    public override void Enter()
    {
        base.Enter();
        refresh();
    }

    private void refresh()
    {
        content.Clear();

        var club = api.User.Value.Club;

        if (club == null)
        {
            content.Child = new OnlineErrorContainer("You are not in a club.")
            {
                Text = "Join a club to see this tab.",
                Anchor = Anchor.TopCentre,
                Origin = Anchor.Centre,
                ShowInstantly = true,
                Y = 200
            };
            return;
        }

        var req = new ClubRequest(club.ID);
        req.Success += res => content.Child = createContent(res.Data!);
        api.PerformRequestAsync(req);
    }

    private Drawable createContent(APIClub club) => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Direction = FillDirection.Vertical,
        Children = new Drawable[]
        {
            new DashboardClubHeader(club),
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Top = 24, Horizontal = 16 },
                Spacing = new Vector2(24),
                Children = new Drawable[]
                {
                    new DashboardItemList<APIUser>("Online Members", club.Members!.Where(x => x.IsOnline).ToList(), u => new DrawableUserCard(u) { Width = 338 }),
                    new DashboardItemList<APIUser>("Offline Members", club.Members!.Where(x => !x.IsOnline).ToList(), u => new DrawableUserCard(u) { Width = 338 }),
                }
            }
        }
    };
}
