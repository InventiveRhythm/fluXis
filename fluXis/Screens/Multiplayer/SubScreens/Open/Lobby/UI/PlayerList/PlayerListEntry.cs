using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface;
using fluXis.Online.API.Models.Multi;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;

public partial class PlayerListEntry : Container
{
    public MultiplayerParticipant Participant { get; }

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
