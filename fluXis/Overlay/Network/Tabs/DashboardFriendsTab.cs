using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Social;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Social;
using fluXis.Online.Drawables;
using fluXis.Online.Drawables.Users;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Network.Tabs.Shared;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardFriendsTab : DashboardTab
{
    public override LocalisableString Title => LocalizationStrings.Dashboard.Friends;
    public override IconUsage Icon => FontAwesome6.Solid.UserGroup;
    public override DashboardTabType Type => DashboardTabType.Friends;

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

        var req = new FriendsRequest();
        req.Success += res => content.Child = createContent(res.Data!);
        req.Failure += ex => content.Child = new OnlineErrorContainer
        {
            Text = ex.Message,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.Centre,
            ShowInstantly = true,
            Y = 200
        };
        api.PerformRequestAsync(req);
    }

    private static FillFlowContainer createContent(APIFriends friends)
    {
        var children = new List<Drawable>();

        if (friends.Rooms.Count > 0)
            children.Add(new DashboardItemList<MultiplayerRoom>("Active Lobbies", friends.Rooms, r => new DrawableMultiplayerCard(r) { Width = 348 }));

        children.AddRange(new Drawable[]
        {
            new DashboardItemList<APIUser>("Online", friends.Users.Where(x => x.IsOnline).ToList(), u => new DrawableUserCard(u) { Width = 348 }),
            new DashboardItemList<APIUser>("Offline", friends.Users.Where(x => !x.IsOnline).ToList(), u => new DrawableUserCard(u) { Width = 348 })
        });

        return new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(24),
            Children = children
        };
    }
}
