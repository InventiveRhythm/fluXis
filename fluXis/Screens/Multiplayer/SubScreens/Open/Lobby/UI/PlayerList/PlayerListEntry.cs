using System.Collections.Generic;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.Multiplayer;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;

public partial class PlayerListEntry : Container, IHasContextMenu
{
    public MenuItem[] ContextMenuItems
    {
        get
        {
            var list = new List<MenuItem>
            {
                new FluXisMenuItem("View Profile", FontAwesome6.Solid.User, () => game?.PresentUser(Participant.ID))
            };

            if (client.Room.Host.ID == client.Player.ID && Participant.ID != client.Player.ID)
                list.Add(new FluXisMenuItem("Promote to Host", FontAwesome6.Solid.Crown, MenuItemType.Normal, () => client.TransferHost(Participant.ID)));

            return list.ToArray();
        }
    }

    public MultiplayerParticipant Participant { get; }

    [Resolved]
    private MultiplayerClient client { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    private PlayerListEntryContent content;
    private LoadingIcon loadingIcon;

    public PlayerListEntry(MultiplayerParticipant participant)
    {
        Participant = participant;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 80;

        Children = new Drawable[]
        {
            new LoadWrapper<PlayerListEntryContent>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => content = new PlayerListEntryContent(Participant),
                OnComplete = d =>
                {
                    d.FadeInFromZero(400);
                    loadingIcon.FadeOut(400);
                }
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(30)
            }
        };
    }

    public void SetState(MultiplayerUserState state)
    {
        if (content is null)
        {
            Schedule(() => SetState(state));
            return;
        }

        if (!content.IsLoaded)
        {
            content.OnLoadComplete += _ => content.SetState(state);
            return;
        }

        content.SetState(state);
    }
}
