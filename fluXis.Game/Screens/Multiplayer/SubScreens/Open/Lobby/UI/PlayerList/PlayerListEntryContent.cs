using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Shared.Components.Multi;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;

public partial class PlayerListEntryContent : CompositeDrawable
{
    private MultiplayerParticipant participant { get; }

    private SpriteIcon readyIcon;

    public PlayerListEntryContent(MultiplayerParticipant participant)
    {
        this.participant = participant;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        participant.Resolve();

        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new DrawableAvatar(participant.User)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Size = new Vector2(50)
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = 50 },
                Children = new Drawable[]
                {
                    new DrawableBanner(participant.User)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = 0.3f
                    },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Padding = new MarginPadding { Left = 10 },
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = participant.User.Username,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                FontSize = 30
                            }
                        }
                    },
                    readyIcon = new SpriteIcon
                    {
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Icon = FontAwesome6.Solid.Check,
                        Size = new Vector2(20),
                        Shadow = true,
                        Margin = new MarginPadding { Right = 10 },
                        Alpha = 0,
                        Colour = Colour4.FromHex("#7BE87B")
                    }
                }
            }
        };
    }

    public void SetState(MultiplayerUserState state)
    {
        readyIcon.FadeTo(state == MultiplayerUserState.Ready ? 1 : 0, 200);
    }
}
