using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Overlay.Settings.Sections;
using fluXis.Overlay.Settings.Sidebar;
using fluXis.Overlay.Settings.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Overlay.Settings;

public partial class SettingsMenu : IconEntranceOverlay, IKeyBindingHandler<FluXisGlobalKeybind>
{
    protected override float OverlayPadding => 96;
    protected override ColourInfo BackgroundColor => Theme.Background1;
    protected override IconUsage Icon => FontAwesome6.Solid.Gear;

    private Bindable<SettingsSection> currentSection { get; } = new();

    private List<SettingsSection> sections { get; } = new()
    {
        new GeneralSection(),
        new AppearanceSection(),
        new InputSection(),
        new UserInterfaceSection(),
        new GameplaySection(),
        new AudioSection(),
        new GraphicsSection(),
        new PluginsSection(),
        new MaintenanceSection(),
        new ExperimentsSection()
    };

    private SettingsCategorySelector categorySelector;
    private FluXisScrollContainer scrollContainer;

    private Sample tabSwitch;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        OpenSample = samples.Get("UI/Settings/open");
        CloseSample = samples.Get("UI/Settings/close");
        tabSwitch = samples.Get("UI/Settings/change-tab");
    }

    protected override Drawable[] CreateContent() => new Drawable[]
    {
        new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = new Dimension[]
            {
                new(GridSizeMode.AutoSize),
                new()
            },
            Content = new[]
            {
                new Drawable[]
                {
                    categorySelector = new SettingsCategorySelector(sections, currentSection) { CloseAction = Hide }
                },
                new Drawable[]
                {
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new Dimension[]
                        {
                            new(GridSizeMode.Absolute, 300),
                            new()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new SettingsSidebar(currentSection)
                                {
                                    ScrollToSection = s => scrollContainer.ScrollTo(s)
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding(48)
                                    {
                                        Top = 20,
                                        Right = 32
                                    },
                                    Masking = true,
                                    Child = scrollContainer = new FluXisScrollContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        ScrollbarAnchor = Anchor.TopRight,
                                        Masking = false,
                                        Child = new Container<SettingsSection>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Padding = new MarginPadding { Right = 20 },
                                            ChildrenEnumerable = sections
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        },
        new CheatCodeHandler(showExperiments, Key.W, Key.S, Key.D, Key.A, Key.W)
    };

    protected override void LoadComplete()
    {
        currentSection.BindValueChanged(sectionChanged);

        if (sections.Count > 0)
            currentSection.Value = sections[0];

        base.LoadComplete();
    }

    private void showExperiments()
    {
        var tab = categorySelector.ExperimentsTab;
        var section = sections.OfType<ExperimentsSection>().FirstOrDefault();

        if (tab != null && section != null)
        {
            tab.FadeInFromZero(200);
            currentSection.Value = section;
        }
    }

    private void sectionChanged(ValueChangedEvent<SettingsSection> e)
    {
        scrollContainer.ScrollToStart();

        var prev = e.OldValue;
        var next = e.NewValue;

        if (prev != null)
        {
            int index = sections.IndexOf(next);
            int previousIndex = sections.IndexOf(prev);

            prev.FadeOut(200);

            if (previousIndex > index)
            {
                next.X = -60;
                prev.MoveToX(60, 400, Easing.OutQuint);
            }
            else if (previousIndex < index)
            {
                next.X = 60;
                prev.MoveToX(-60, 400, Easing.OutQuint);
            }
        }

        next.MoveToX(0, 400, Easing.OutQuint).FadeIn(200);

        if (prev != null && !InitialAnimation)
            tabSwitch?.Play();
    }

    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;
    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnMouseDown(MouseDownEvent e) => true;

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        var keyStr = e.Key.ToString();

        if (!keyStr.StartsWith("Number")) return true;

        keyStr = keyStr.Replace("Number", "");

        if (!int.TryParse(keyStr, out var num)) return true;

        if (num == 0) num = 10;

        var tab = sections.ElementAtOrDefault(num - 1);

        if (tab != null)
            currentSection.Value = tab;

        return true;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back or FluXisGlobalKeybind.ToggleSettings:
                Hide();
                break;
        }

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
