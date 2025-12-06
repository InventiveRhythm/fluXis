using System;
using System.Linq;
using fluXis.Graphics.UserInterface;
using fluXis.Mods;
using fluXis.Screens.Select.Mods;
using NUnit.Framework;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Testing;
using osuTK;
using osuTK.Input;

namespace fluXis.Tests.Select;

public partial class TestModSelect : FluXisManualInputTestScene
{
    private bool cached = false;
    private ModsOverlay mods;

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        Clear();

        if (!cached)
            CreateClock();

        cached = true;

        Add(mods = new ModsOverlay());
    });

    [Test]
    public void TestVisibility()
    {
        showAndWait();
        hideAndWait();
    }

    [Test]
    public void TestSelectAllMisc()
    {
        ModEntry[] list = null!;

        showAndWait();
        AddStep("select all misc mods", () =>
        {
            list = mods.ChildrenOfType<ModEntry>().Where(x => x.Mod.Type == ModType.Misc).ToArray();

            foreach (var entry in list)
            {
                Input.MoveMouseTo(entry);
                Input.Click(MouseButton.Left);
            }
        });
        AddAssert("all selected", () => list.All(x => mods.SelectedMods.Contains(x.Mod)));
    }

    [Test]
    public void TestChangeRate()
    {
        ModSelectRate rate = null!;
        SliderBar<float> slider = null!;
        ModSelectRate.SliderTickMark mark = null!;

        showAndWait();
        AddStep("change rate", () =>
        {
            rate = mods.ChildrenOfType<ModSelectRate>().First();
            slider = rate.ChildrenOfType<SliderBar<float>>().First();
            mark = rate.ChildrenOfType<ModSelectRate.SliderTickMark>().First(x => Math.Abs(x.Value - 1f) < 0.01f);

            Input.MoveMouseTo(slider, new Vector2(250, 0));
            Input.Click(MouseButton.Left);
        });
        AddAssert("mods has rate", () => mods.SelectedMods.Any(x => x is RateMod));
        AddStep("move to reset", () =>
        {
            var x = mark.ScreenSpaceDrawQuad.Centre.X;
            var y = slider.ScreenSpaceDrawQuad.Centre.Y;
            Input.MoveMouseTo(new Vector2(x, y));
            Input.Click(MouseButton.Left);
        });
        AddAssert("mods no longer has rate", () => !mods.SelectedMods.Any(x => x is RateMod));
    }

    [Test]
    public void TestDeselectAll()
    {
        showAndWait();
        AddStep("select 4 mods", () =>
        {
            foreach (var entry in mods.ChildrenOfType<ModEntry>().Take(4))
            {
                Input.MoveMouseTo(entry);
                Input.Click(MouseButton.Left);
            }
        });
        AddStep("change rate", () =>
        {
            var rate = mods.ChildrenOfType<ModSelectRate>().First();
            var slider = rate.ChildrenOfType<FluXisSlider<float>>().First();

            Input.MoveMouseTo(slider);
            Input.PressButton(MouseButton.Left);
            Input.MoveMouseTo(slider, new Vector2(250, 0));
            Input.ReleaseButton(MouseButton.Left);
        });
        AddStep("deselect all", () => mods.DeselectAll());
        AddAssert("none selected", () => mods.SelectedMods.Count == 0);
    }

    [Test]
    public void TestDeselectIncompatible()
    {
        ModEntry mod = null!;

        showAndWait();
        AddStep("select first mod", () =>
        {
            Input.MoveMouseTo(mod = mods.ChildrenOfType<ModEntry>().FirstOrDefault(x => x.Mod.IncompatibleMods.Length != 0));
            Input.Click(MouseButton.Left);
        });
        AddAssert("first is selected", () => mods.SelectedMods.Contains(mod.Mod));
        AddAssert("mod is not null", () => mod != null);
        AddStep("select first incompatible", () =>
        {
            Input.MoveMouseTo(mods.ChildrenOfType<ModEntry>().FirstOrDefault(x => x.Mod.GetType() == mod.Mod.IncompatibleMods[0]));
            Input.Click(MouseButton.Left);
        });
        AddAssert("first is deselected", () => !mods.SelectedMods.Contains(mod.Mod));
    }

    [Test]
    public void TestIgnoreClickWhenHidden()
    {
        showAndWait();
        AddAssert("list is empty", () => mods.SelectedMods.Count == 0);
        AddStep("select mod", () =>
        {
            Input.MoveMouseTo(mods.ChildrenOfType<ModEntry>().ElementAt(0));
            Input.Click(MouseButton.Left);
        });
        AddAssert("list is not empty", () => mods.SelectedMods.Count != 0);
        AddStep("clear", () => mods.DeselectAll());
        hideAndWait();
        AddStep("select mod", () =>
        {
            Input.MoveMouseTo(mods.ChildrenOfType<ModEntry>().ElementAt(0));
            Input.Click(MouseButton.Left);
        });
        AddAssert("list is empty", () => mods.SelectedMods.Count == 0);
    }

    private void showAndWait()
    {
        AddStep("show overlay", () => mods.Show());
        AddWaitStep("wait to show", 4);
    }

    private void hideAndWait()
    {
        AddStep("hide overlay", () => mods.Hide());
        AddWaitStep("wait to hide", 4);
    }
}
