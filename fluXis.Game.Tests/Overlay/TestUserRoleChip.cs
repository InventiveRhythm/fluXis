using fluXis.Game.Overlay.User.Header;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Tests.Overlay;

public partial class TestUserRoleChip : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        const int min_role = 0;
        const int max_role = 5;

        var flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new osuTK.Vector2(5),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };

        for (int i = min_role; i <= max_role; i++)
        {
            flow.Add(new HeaderRoleChip
            {
                RoleId = i
            });
        }

        Add(flow);
    }
}
