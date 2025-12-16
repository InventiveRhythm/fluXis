using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface.Context;
using fluXis.Online.API.Models.Multi;
using fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI;

public partial class MultiLobbyPlayerList : FluXisContextMenuContainer
{
    public MultiplayerRoom Room { get; set; }

    private FillFlowContainer<PlayerListEntry> playerList;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientHorizontal(Colour4.Black.Opacity(.8f), Colour4.Black.Opacity(0))
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 48, Top = 136, Bottom = 24 },
                Children = new Drawable[]
                {
                    new FluXisScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ScrollbarVisible = false,
                        Child = playerList = new FillFlowContainer<PlayerListEntry>
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding(20),
                            Spacing = new Vector2(20)
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        foreach (var participant in Room.Participants)
        {
            playerList.Add(new PlayerListEntry(participant));
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
