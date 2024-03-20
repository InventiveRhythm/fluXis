using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Shared.Components.Multi;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;

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
        Height = 50;
        Masking = true;
        CornerRadius = 10;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.5f
            },
            new LoadWrapper<PlayerListEntryContent>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => content = new PlayerListEntryContent(Participant),
                OnComplete = d =>
                {
                    d.FadeInFromZero(200);
                    loadingIcon.FadeOut(200);
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

    public void SetState(MultiplayerUserState state) => content.SetState(state);
}
