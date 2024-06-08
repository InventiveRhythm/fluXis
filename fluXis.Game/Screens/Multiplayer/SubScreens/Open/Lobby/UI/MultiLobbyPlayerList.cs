using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;
using JetBrains.Annotations;
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
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = playerList = new FillFlowContainer<PlayerListEntry>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(20),
                    Spacing = new Vector2(20)
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        foreach (var participant in Room.Participants)
        {
            var impl = participant as MultiplayerParticipant;
            playerList.Add(new PlayerListEntry(impl));
        }
    }

    [CanBeNull]
    public PlayerListEntry GetPlayer(long id)
    {
        return playerList.Children.FirstOrDefault(p => p.Participant.ID == id);
    }

    public void AddPlayer(MultiplayerParticipant participant)
    {
        playerList.Add(new PlayerListEntry(participant));
    }

    public void RemovePlayer(long id)
    {
        playerList.Children.FirstOrDefault(p => p.Participant.ID == id)?.Expire();
    }
}
