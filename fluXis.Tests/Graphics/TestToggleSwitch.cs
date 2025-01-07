using fluXis.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Tests.Graphics;

public partial class TestToggleSwitch : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var state = new BindableBool();

        var toggle = new FluXisToggleSwitch
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            State = state
        };

        Add(toggle);

        AddStep("Toggle", () => state.Value = !state.Value);
        AddStep("Disable", () => state.Value = false);
        AddStep("Enable", () => state.Value = true);
    }
}
