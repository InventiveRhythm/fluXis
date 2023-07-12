using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Ranked;

public partial class MultiRankedMain : MultiSubScreen
{
    public override string Title => "Ranked";
    public override string SubTitle => "Main Menu";

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
}
