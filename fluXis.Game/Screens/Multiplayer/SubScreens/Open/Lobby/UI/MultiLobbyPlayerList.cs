using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Scroll;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Multi;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI;

public partial class MultiLobbyPlayerList : MultiLobbyContainer
{
    public MultiplayerRoom Room { get; set; }

    private FillFlowContainer<PlayerListEntry> playerList;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Children = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "Players",
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                FontSize = 30
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = 30 },
                Child = new FluXisScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Child = playerList = new FillFlowContainer<PlayerListEntry>
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 10)
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        foreach (var user in Room.Users)
        {
            playerList.Add(new PlayerListEntry { User = user });
        }
    }

    public void AddPlayer(APIUserShort user)
    {
        playerList.Add(new PlayerListEntry { User = user });
    }

    public void RemovePlayer(int id)
    {
        playerList.Children.FirstOrDefault(p => p.User.ID == id)?.Expire();
    }
}
