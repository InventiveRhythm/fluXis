using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Panel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiLobby : MultiSubScreen
{
    public override string Title => "Open Match";
    public override string SubTitle => "Lobby";

    [Resolved]
    private FluXisGameBase game { get; set; }

    private bool confirmExit;

    [BackgroundDependencyLoader]
    private void load()
    {
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (confirmExit) return base.OnExiting(e);

        game.Overlay ??= new ButtonPanel
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = "Are you sure you want to exit the lobby?",
            Buttons = new[]
            {
                new ButtonData
                {
                    Text = "Leave",
                    Color = FluXisColors.ButtonRed,
                    Action = () =>
                    {
                        confirmExit = true;
                        this.Exit();
                    }
                },
                new ButtonData { Text = "Stay" }
            }
        };

        return true;
    }
}
