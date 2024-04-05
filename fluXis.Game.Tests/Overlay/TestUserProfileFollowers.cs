using fluXis.Game.Overlay.User.Sidebar;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Tests.Overlay;

public partial class TestUserProfileFollowers : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Add(new Container
        {
            Width = 300,
            AutoSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Child = new ProfileFollowerList(1)
        });
    }
}
