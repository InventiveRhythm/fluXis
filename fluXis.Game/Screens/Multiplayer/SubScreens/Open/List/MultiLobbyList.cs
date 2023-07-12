using fluXis.Game.Graphics;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.List.UI;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List;

public partial class MultiLobbyList : MultiSubScreen
{
    public override string Title => "Open Match";
    public override string SubTitle => "Lobby List";

    private FillFlowContainer lobbyList;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            lobbyList = new FillFlowContainer
            {
                Width = 1320,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(20),
                Direction = FillDirection.Full,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new ClickableContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Action = () => this.Push(new MultiLobby()),
                Child = new FluXisSpriteText { Text = "Enter lobby screen" }
            }
        };
    }

    protected override void LoadComplete()
    {
        for (var i = 0; i < 12; i++)
            lobbyList.Add(new EmptyLobbySlot());

        base.LoadComplete();
    }
}
