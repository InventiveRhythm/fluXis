using fluXis.Graphics.UserInterface;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osuTK;
using osuTK.Input;

namespace fluXis.Tests.Graphics;

public partial class TestToggleSwitch : FluXisManualInputTestScene
{
    protected override bool DisplayCursor => true;

    private FluXisToggleSwitch toggle;
    private BindableBool state;

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        Clear();

        state = new BindableBool();

        Add(toggle = new FluXisToggleSwitch
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            State = state
        });
    });

    [Test]
    public void TestButtonClick()
    {
        AddStep("click", () =>
        {
            Input.MoveMouseTo(toggle);
            Input.Click(MouseButton.Left);
        });

        AddAssert("is true", () => state.Value);

        AddStep("click", () =>
        {
            Input.MoveMouseTo(toggle);
            Input.Click(MouseButton.Left);
        });

        AddAssert("is false", () => !state.Value);
    }

    [Test]
    public void TestNubDrag()
    {
        AddStep("move mouse to left", () => Input.MoveMouseTo(toggle, new Vector2(-24, 0)));
        AddStep("press mouse", () => Input.PressButton(MouseButton.Left));
        AddStep("move mouse to right", () => Input.MoveMouseTo(toggle, new Vector2(24, 0)));
        AddStep("release mouse", () => Input.ReleaseButton(MouseButton.Left));

        AddAssert("is true", () => state.Value);

        AddStep("press mouse", () => Input.PressButton(MouseButton.Left));
        AddStep("move mouse to left", () => Input.MoveMouseTo(toggle, new Vector2(-24, 0)));
        AddStep("release mouse", () => Input.ReleaseButton(MouseButton.Left));

        AddAssert("is false", () => !state.Value);
    }
}
