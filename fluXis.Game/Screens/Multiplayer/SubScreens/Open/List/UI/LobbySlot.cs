using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Drawables.Online;
using fluXis.Game.Online.API.Models.Multi;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List.UI;

public partial class LobbySlot : Container
{
    public MultiLobbyList List { get; init; }
    public MultiplayerRoom Room { get; init; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 650;
        Height = 90;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background4
            },
            new LoadWrapper<DrawableOnlineBackground>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => new DrawableOnlineBackground(Room.Map),
                OnComplete = d => d.FadeInFromZero(400)
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
                Alpha = .5f
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(20),
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    Width = 300,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Spacing = new Vector2(-2),
                    Children = new Drawable[]
                    {
                        new TruncatingText
                        {
                            Text = Room.Settings.Name,
                            RelativeSizeAxes = Axes.X,
                            WebFontSize = 24
                        },
                        new TruncatingText
                        {
                            Text = $"hosted by {Room.Host.Username}",
                            RelativeSizeAxes = Axes.X,
                            WebFontSize = 14
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        List.JoinLobby(Room.RoomID);
        return true;
    }
}
