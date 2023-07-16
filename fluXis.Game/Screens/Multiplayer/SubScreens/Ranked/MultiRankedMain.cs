using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Ranked;

public partial class MultiRankedMain : MultiSubScreen
{
    public override string Title => "Ranked";
    public override string SubTitle => "Main Menu";

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new FluXisSpriteText
        {
            Text = "Nothing here yet!",
            FontSize = 40,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Shadow = true
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 0);
        base.OnEntering(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 0);
        base.OnResuming(e);
    }
}
