using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.Drawables;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;

public partial class PlayerListEntryContent : CompositeDrawable
{
    private MultiplayerParticipant participant { get; }

    private Box stateBackground;
    private FluXisSpriteText stateText;

    public PlayerListEntryContent(MultiplayerParticipant participant)
    {
        this.participant = participant;
    }

    [BackgroundDependencyLoader]
    private void load(UserCache users)
    {
        participant.User = users.Get(participant.ID);

        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(80),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                CornerRadius = 10,
                Masking = true,
                Child = new DrawableAvatar(participant.User)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = 90 },
                Direction = FillDirection.Vertical,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Children = new Drawable[]
                {
                    participant.User.NamePaint is not null
                        ? new GradientText
                        {
                            Text = participant.User.Username,
                            WebFontSize = 20,
                            Colour = participant.User.NamePaint.Colors.CreateColorInfo()
                        }
                        : new FluXisSpriteText
                        {
                            Text = participant.User.Username,
                            WebFontSize = 20
                        },
                    new CircularContainer
                    {
                        Size = new Vector2(100, 20),
                        Masking = true,
                        Children = new Drawable[]
                        {
                            stateBackground = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background1
                            },
                            stateText = new FluXisSpriteText
                            {
                                Text = "Not Ready",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                WebFontSize = 12,
                                Alpha = .75f
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        SetState(participant.State);
    }

    public void SetState(MultiplayerUserState state)
    {
        stateBackground.Colour = state switch
        {
            MultiplayerUserState.Ready => Colour4.FromHex("#57FF57"),
            MultiplayerUserState.Playing => Colour4.FromHex("#FFC657"),
            MultiplayerUserState.Finished => Colour4.FromHex("#FFC657"),
            MultiplayerUserState.Results => Colour4.FromHex("#57C7FF"),
            _ => FluXisColors.Background1
        };

        stateText.Text = state switch
        {
            MultiplayerUserState.Ready => "Ready",
            MultiplayerUserState.Playing => "Playing",
            MultiplayerUserState.Finished => "Playing", // they are just waiting for the results
            MultiplayerUserState.Results => "Results",
            _ => "Not Ready"
        };

        stateText.Colour = FluXisColors.IsBright(stateBackground.Colour) ? Colour4.Black : FluXisColors.Text;
    }
}
