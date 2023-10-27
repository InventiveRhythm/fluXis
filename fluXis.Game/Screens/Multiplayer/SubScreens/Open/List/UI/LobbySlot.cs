using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API.Models.Multi;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List.UI;

public partial class LobbySlot : Container
{
    public MultiLobbyList List { get; set; }
    public MultiplayerRoom Room { get; set; }

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
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = Room.Settings.Name,
                            RelativeSizeAxes = Axes.X,
                            Truncate = true,
                            FontSize = 30
                        },
                        new FluXisSpriteText
                        {
                            Text = $"hosted by {Room.Host.Username}",
                            RelativeSizeAxes = Axes.X,
                            Truncate = true,
                            FontSize = 20
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        List.JoinLobby(Room);
        return true;
    }
}
