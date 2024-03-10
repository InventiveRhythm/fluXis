using fluXis.Game.Online;
using fluXis.Game.Online.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Tests.Online;

public partial class TestDrawableUserCard : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10)
        };

        Add(flow);

        AddStep("Add uid 1", () => flow.Add(new DrawableUserCard(UserCache.GetUser(1))
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }));

        AddStep("Add uid 2", () => flow.Add(new DrawableUserCard(UserCache.GetUser(2))
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }));
    }
}
