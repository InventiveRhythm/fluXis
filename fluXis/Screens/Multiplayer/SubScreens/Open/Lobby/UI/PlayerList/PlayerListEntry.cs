using System.Collections.Generic;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.Drawables.Images;
using fluXis.Online.Multiplayer;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
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
                new MenuActionItem("View Profile", FontAwesome6.Solid.User, () => game?.PresentUser(Participant.ID))
            };

            if (client.Room!.Host.ID == client.Player.ID && Participant.ID != client.Player.ID)
                list.Add(new MenuActionItem("Promote to Host", FontAwesome6.Solid.Crown, MenuItemType.Normal, () => client.TransferHost(Participant.ID)));

            return list.ToArray();
        }
    }

    public MultiplayerParticipant Participant { get; }

    [Resolved]
    private MultiplayerClient client { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    private FluXisSpriteIcon hostCrown;
    private Box stateBackground;
    private FluXisSpriteText stateText;

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
            new LoadWrapper<DrawableAvatar>
            {
                Size = new Vector2(80),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                CornerRadius = 10,
                Masking = true,
                LoadContent = () => new DrawableAvatar(Participant.Player)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            },
            hostCrown = new FluXisSpriteIcon
            {
                Position = new Vector2(8, -4),
                Icon = FontAwesome6.Solid.Crown,
                Origin = Anchor.Centre,
                Size = new Vector2(32),
                Rotation = -14,
                Alpha = 0
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
                    Participant.Player.NamePaint is not null
                        ? new GradientText
                        {
                            Text = Participant.Player.Username,
                            WebFontSize = 20,
                            Colour = Participant.Player.NamePaint.Colors.CreateColorInfo()
                        }
                        : new FluXisSpriteText
                        {
                            Text = Participant.Player.Username,
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
                                Colour = Theme.Background1
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
        SetState(Participant.State);
    }

    protected override void Update()
    {
        base.Update();

        hostCrown.Alpha = client.Room?.Host.ID == Participant.ID ? 1 : 0;
    }

    public void SetState(MultiplayerUserState state)
    {
        if (!IsLoaded)
        {
            Schedule(() => SetState(state));
            return;
        }

        stateBackground.Colour = state switch
        {
            MultiplayerUserState.Ready => Colour4.FromHex("#57FF57"),
            MultiplayerUserState.Playing => Colour4.FromHex("#FFC657"),
            MultiplayerUserState.Finished => Colour4.FromHex("#FFC657"),
            MultiplayerUserState.Results => Colour4.FromHex("#57C7FF"),
            _ => Theme.Background1
        };

        stateText.Text = state switch
        {
            MultiplayerUserState.Ready => "Ready",
            MultiplayerUserState.Playing => "Playing",
            MultiplayerUserState.Finished => "Playing", // they are just waiting for the results
            MultiplayerUserState.Results => "Results",
            _ => "Not Ready"
        };

        stateText.Colour = Theme.IsBright(stateBackground.Colour) ? Colour4.Black : Theme.Text;
    }
}
