using System;
using fluXis.Database.Maps;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Form;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Tests.Graphics;

public partial class TestFormComponents : FluXisTestScene
{
    private Container content;

    [SetUp]
    public void Setup()
    {
        Schedule(() =>
        {
            Clear();
            Add(new Box { RelativeSizeAxes = Axes.Both, Colour = Theme.Background2 });
            Add(content = new PopoverContainer { RelativeSizeAxes = Axes.Both });
        });
    }

    [Test]
    public void TestInput()
    {
        AddStep("Add Components", () => content.Add(new FillFlowContainer
        {
            Width = 380,
            AutoSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(12),
            Children =
            [
                new FormInput("Test Input", new Bindable<string>("waow")),
                new FormSlider<double>("Test Slider", 1, 0, 10),
                new FormCheckbox("Test Check", new Bindable<bool>()),
                new FormColor("Test Color", new Bindable<Colour4>(Color4.White)),
                new FormDropdown<MapStatus>("Test Dropdown", new Bindable<MapStatus>(), Enum.GetValues<MapStatus>()),
                new FormBasicFilePicker("Test File", "")
            ]
        }));
    }
}
