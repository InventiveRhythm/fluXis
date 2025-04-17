using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Social;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Social;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardFriendsTab : DashboardTab
{
    public override string Title => "Friends";
    public override IconUsage Icon => FontAwesome6.Solid.UserGroup;
    public override DashboardTabType Type => DashboardTabType.Friends;

    [Resolved]
    private IAPIClient api { get; set; }

    private FluXisScrollContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        Header.Child = new FluXisButton
        {
            Text = "Refresh",
            FontSize = 20,
            Size = new Vector2(144, 36),
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            Margin = new MarginPadding { Right = 20 },
            Color = Colour4.Transparent,
            Action = refresh
        };

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
            children.Add(new ItemList<MultiplayerRoom>("Active Lobbies", friends.Rooms, r => new DrawableMultiplayerCard(r) { Width = 348 }));

        children.AddRange(new Drawable[]
        {
            new ItemList<APIUser>("Online", friends.Users.Where(x => x.IsOnline).ToList(), u => new DrawableUserCard(u) { Width = 348 }),
            new ItemList<APIUser>("Offline", friends.Users.Where(x => !x.IsOnline).ToList(), u => new DrawableUserCard(u) { Width = 348 })
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

    private partial class ItemList<T> : FillFlowContainer
    {
        private string title { get; }
        private IList<T> items { get; }
        private Func<T, Drawable> create { get; }

        public ItemList(string title, IList<T> items, Func<T, Drawable> create)
        {
            this.create = create;
            this.title = title;
            this.items = items;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(items.Any() ? 12 : 4);

            InternalChildren = new Drawable[]
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
                            Text = title,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            WebFontSize = 20
                        },
                        new FluXisSpriteText
                        {
                            Text = $"{items.Count}",
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            WebFontSize = 14,
                            Alpha = .6f
                        }
                    }
                },
                new FluXisSpriteText
                {
                    Text = "Nobody here...",
                    WebFontSize = 14,
                    Alpha = items.Any() ? 0f : .8f
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Full,
                    Spacing = new Vector2(8),
                    Alpha = items.Any() ? 1f : 0f,
                    ChildrenEnumerable = items.Select(create)
                }
            };
        }
    }
}
