using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Gamepad;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Input;
using fluXis.Overlay.Settings.Sections;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
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

    [CanBeNull]
    public Action<Drawable> ScrollToItem { get; set; }

    public Bindable<string> SearchTerm { get; set; } = new();
    private FluXisTextBox searchBox;

    private Drawable gamepadInfo;

    public SettingsCategorySelector(List<SettingsSection> list, Bindable<SettingsSection> bind)
    {
        sections = list;
        currentSection = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = 48 + 12 },
                Child = new FluXisScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ScrollbarVisible = false,
                    ScrollbarOverlapsContent = true,
                    Child = new FillFlowContainer<SettingsCategoryTab>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(16),
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(4),
                        ChildrenEnumerable = sections.Select(createTab)
                    },
                },
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 48 + 12,
                Padding = new MarginPadding { Horizontal = 12, Top = 12 },
                Child = searchBox = new FluXisTextBox
                {
                    RelativeSizeAxes = Axes.Both,
                    OnTextChanged = () => SearchTerm.Value = searchBox.Text,
                    PlaceholderText = "Click to search...",
                    FontSize = FluXisSpriteText.GetWebFontSize(16),
                    BackgroundActive = Theme.Background2,
                    BorderColour = Theme.Background4,
                    BorderThickness = 3,
                    SidePadding = 12
                },
            },
            gamepadInfo = new FillFlowContainer()
            {
                RelativeSizeAxes = Axes.X,
                Height = 120,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(12),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Children =
                [
                    createGamepadIcon(ButtonGlyph.LeftBump),
                    new FluXisSpriteText
                    {
                        Text = "Change categories",
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        WebFontSize = 14
                    },
                    createGamepadIcon(ButtonGlyph.RightBump),
                ]
            }
        };
    }

    protected override void Update()
    {
        base.Update();
        gamepadInfo.Alpha = GamepadHandler.GamepadConnected ? 1f : 0f;
    }

    private GamepadIcon createGamepadIcon(ButtonGlyph glyph) => new(glyph)
    {
        Size = new Vector2(32),
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre
    };

    private SettingsCategoryTab createTab(SettingsSection s)
    {
        var tab = new SettingsCategoryTab(s, currentSection);
        tab.ScrollToItem = ScrollToItem;

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
