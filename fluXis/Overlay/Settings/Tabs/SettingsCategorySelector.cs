using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Gamepad;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Overlay.Settings.Sections;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Settings.Tabs;

public partial class SettingsCategorySelector : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    private List<SettingsSection> sections { get; }
    private Bindable<SettingsSection> currentSection { get; }

    public SettingsCategoryTab ExperimentsTab { get; private set; }
    public Action CloseAction { get; init; }

    private GamepadIcon leftIcon;
    private GamepadIcon rightIcon;

    public SettingsCategorySelector(List<SettingsSection> list, Bindable<SettingsSection> bind)
    {
        sections = list;
        currentSection = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 90;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background3
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding(10),
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                ChildrenEnumerable = (leftIcon = createGamepadIcon(ButtonGlyph.LeftBump))
                                     .Yield<Drawable>()
                                     .Concat(sections.Select(createTab))
                                     .Concat((rightIcon = createGamepadIcon(ButtonGlyph.RightBump, true)).Yield<Drawable>())
            },
            new IconButton
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Icon = FontAwesome6.Solid.XMark,
                ButtonSize = 70,
                Action = CloseAction,
                Margin = new MarginPadding(10)
            }
        };
    }

    protected override void Update()
    {
        base.Update();
        leftIcon.Alpha = rightIcon.Alpha = GamepadHandler.GamepadConnected ? 1f : 0f;
    }

    private GamepadIcon createGamepadIcon(ButtonGlyph glyph, bool right = false) => new(glyph)
    {
        Size = new Vector2(32),
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,
        Margin = new MarginPadding { Left = right ? 8 : 16, Right = right ? 0 : 8 }
    };

    private SettingsCategoryTab createTab(SettingsSection s)
    {
        var tab = new SettingsCategoryTab(s, currentSection);

        if (s is ExperimentsSection)
        {
            tab.Alpha = 0;
            ExperimentsTab = tab;
        }

        return tab;
    }

    private void switchTab(int by)
    {
        var visible = sections.Where(x => x is not ExperimentsSection || !(ExperimentsTab.Alpha <= 0)).ToList();

        var idx = visible.IndexOf(currentSection.Value);
        idx += by;

        if (idx < 0)
            idx = visible.Count - 1;
        else if (idx > visible.Count - 1)
            idx = 0;

        currentSection.Value = sections[idx];
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.PreviousGroup:
                switchTab(-1);
                return true;

            case FluXisGlobalKeybind.NextGroup:
                switchTab(1);
                return true;
        }

        return false;
    }

    protected override bool OnJoystickPress(JoystickPressEvent e)
    {
        switch (e.Button)
        {
            case JoystickButton.GamePadLeftShoulder:
                switchTab(-1);
                return true;

            case JoystickButton.GamePadRightShoulder:
                switchTab(1);
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
    }
}
