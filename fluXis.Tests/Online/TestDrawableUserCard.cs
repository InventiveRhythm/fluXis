using fluXis.Online;
using fluXis.Online.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Tests.Online;

public partial class TestDrawableUserCard : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(UserCache users)
    {
        var flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10)
        };

        Add(flow);

        AddStep("Add uid 1", () => flow.Add(new DrawableUserCard(users.Get(1))
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }));

        AddStep("Add uid 2", () => flow.Add(new DrawableUserCard(users.Get(2))
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }));
    }
}
