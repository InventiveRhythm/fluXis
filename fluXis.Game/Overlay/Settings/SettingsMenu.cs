using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Overlay.Settings.Sections;
using fluXis.Game.Overlay.Settings.Sidebar;
using fluXis.Game.Overlay.Settings.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Overlay.Settings;

public partial class SettingsMenu : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    protected override bool PlaySamples => false;

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
        new DebugSection(),
        new ExperimentsSection()
    };

    private ClickableContainer content;
    private SettingsCategorySelector categorySelector;
    private FluXisScrollContainer scrollContainer;

    private Container gearContainer;
    private Container settingsContainer;

    private Sample open;
    private Sample close;
    private Sample tabSwitch;

    private bool initial = true;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        open = samples.Get("UI/Settings/open");
        close = samples.Get("UI/Settings/close");
        tabSwitch = samples.Get("UI/Settings/change-tab");

        InternalChildren = new Drawable[]
        {
            new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Action = Hide,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Alpha = .5f
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding(100),
                Child = content = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    CornerRadius = 20,
                    Masking = true,
                    EdgeEffect = FluXisStyles.ShadowLarge,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background1
                        },
                        gearContainer = new Container
                        {
                            Size = new Vector2(200),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Child = new SpriteIcon
                            {
                                RelativeSizeAxes = Axes.Both,
                                Size = new Vector2(.5f),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Icon = FontAwesome6.Solid.Gear
                            }
                        },
                        settingsContainer = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Child = new GridContainer
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
                                                        Padding = new MarginPadding(50)
                                                        {
                                                            Top = 20,
                                                            Right = 30
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
                            }
                        }
                    }
                }
            },
            new CheatCodeHandler(showExperiments, Key.W, Key.S, Key.D, Key.A, Key.W)
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        currentSection.BindValueChanged(sectionChanged);

        if (sections.Count > 0)
            currentSection.Value = sections[0];

        initial = false;
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

        if (prev != null && !initial)
            tabSwitch?.Play();
    }

    protected override void PopIn()
    {
        if (!initial)
            open?.Play();

        const int size = 200;
        const int scale_duration = 400;

        var widthFactor = size / DrawSize.X;
        var heightFactor = size / DrawSize.Y;

        content.ScaleTo(.8f)
               .ResizeTo(new Vector2(widthFactor, heightFactor))
               .ScaleTo(1, scale_duration, Easing.OutQuint)
               .Delay(scale_duration)
               .ResizeTo(1, 600, Easing.OutQuint);

        gearContainer.FadeIn().RotateTo(-80)
                     .RotateTo(0, scale_duration, Easing.OutQuint)
                     .Delay(scale_duration)
                     .FadeOut(200).RotateTo(80, scale_duration, Easing.OutQuint);

        settingsContainer.FadeOut().Then(scale_duration + 400).FadeIn(200);
        this.FadeInFromZero(200);
    }

    protected override void PopOut()
    {
        if (!initial)
            close?.Play();

        this.FadeOut(200);
        content.ScaleTo(.95f, 400, Easing.OutQuint);
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
