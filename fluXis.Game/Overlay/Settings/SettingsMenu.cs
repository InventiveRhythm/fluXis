using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Panel;
using fluXis.Game.Graphics.Scroll;
using fluXis.Game.Input;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Settings.Sections;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Overlay.Settings;

public partial class SettingsMenu : Container, IKeyBindingHandler<FluXisKeybind>
{
    [Resolved]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    public CategorySelector Selector { get; private set; }
    public Container<SettingsSection> SectionContent { get; private set; }

    private bool visible;
    private ClickableContainer content;
    private PanelBackground background;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

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
                    Alpha = .25f
                },
            },
            content = new ClickableContainer
            {
                Width = 1200,
                Height = 600,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(.95f),
                Rotation = 2,
                CornerRadius = 10,
                Masking = true,
                Children = new Drawable[]
                {
                    background = new PanelBackground
                    {
                        Width = 1.2f,
                        RelativePositionAxes = Axes.X
                    },
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
                                Selector = new CategorySelector { Menu = this }
                            },
                            new Drawable[]
                            {
                                new FluXisScrollContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    ScrollbarAnchor = Anchor.TopRight,
                                    Child = SectionContent = new Container<SettingsSection>
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Padding = new MarginPadding { Horizontal = 50, Vertical = 20 }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        createSection(new GeneralSection());
        createSection(new AppearanceSection());
        createSection(new InputSection());
        createSection(new UserInterfaceSection());
        createSection(new GameplaySection());
        createSection(new AudioSection());
        createSection(new GraphicsSection());
        createSection(new PluginsSection());
        createSection(new MaintenanceSection());
        createSection(new DebugSection());
    }

    private void createSection(SettingsSection section)
    {
        Selector.AddTab(new SettingsCategoryTab(this, section));
        SectionContent.Add(section);
    }

    public void SelectSection(SettingsSection section)
    {
        SectionContent.Children.ForEach(s => s.FadeOut(200));
        section.FadeIn(200);

        int index = SectionContent.IndexOf(section);
        background.MoveToX(index * -0.002f, 600, Easing.OutQuint);
    }

    public void ToggleVisibility()
    {
        visible = !visible;

        if (visible)
            Show();
        else
            Hide();
    }

    public override void Hide()
    {
        visible = false;

        content.ScaleTo(.95f, 400, Easing.OutQuint)
               .RotateTo(2, 400, Easing.OutQuint);

        this.FadeOut(200);
    }

    public override void Show()
    {
        visible = true;

        content.RotateTo(0)
               .ScaleTo(1f, 800, Easing.OutElastic);

        this.FadeIn(200);
    }

    protected override bool OnHover(HoverEvent e)
    {
        return true;
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        var keyStr = e.Key.ToString();

        if (keyStr.StartsWith("Number"))
        {
            keyStr = keyStr.Replace("Number", "");

            if (!int.TryParse(keyStr, out var num)) return true;

            if (num == 0) num = 10;

            var tab = Selector.Tabs.ElementAtOrDefault(num - 1);
            Selector.SelectTab(tab);
        }

        return true;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.Back or FluXisKeybind.ToggleSettings:
                Hide();
                break;
        }

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

    public partial class CategorySelector : Container
    {
        public SettingsMenu Menu { get; init; }

        private CircularContainer line;
        public FillFlowContainer<SettingsCategoryTab> Tabs;

        private SettingsCategoryTab selectedTab;

        private Sample tabSwitch;

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            tabSwitch = samples.Get(@"UI/change-tab");

            Height = 50;
            AutoSizeAxes = Axes.X;
            Content.Origin = Content.Anchor = Anchor.TopCentre;

            InternalChildren = new Drawable[]
            {
                Tabs = new FillFlowContainer<SettingsCategoryTab>
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10, 0)
                },
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Width = 4,
                    Height = 3,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Colour = FluXisColors.Surface2
                },
                new Container
                {
                    Height = 5,
                    RelativeSizeAxes = Axes.X,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Y = 1,
                    Child = line = new CircularContainer
                    {
                        Height = 5,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Masking = true,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.White
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            ScheduleAfterChildren(() => SelectTab());
        }

        public void AddTab(SettingsCategoryTab tab)
        {
            tab.Index = Tabs.Children.Count;
            Tabs.Add(tab);
        }

        public void SelectTab(SettingsCategoryTab tab = null)
        {
            tab ??= Tabs.Children.First();

            // if still null, return
            if (tab == null)
                return;

            if (selectedTab == tab)
                return;

            if (selectedTab != null)
            {
                int index = Tabs.IndexOf(tab);
                int previousIndex = Tabs.IndexOf(selectedTab);

                if (previousIndex > index)
                {
                    tab.Section.X = -60;
                    selectedTab.Section.MoveToX(60, 400, Easing.OutQuint);
                }
                else if (previousIndex < index)
                {
                    tab.Section.X = 60;
                    selectedTab.Section.MoveToX(-60, 400, Easing.OutQuint);
                }
            }
            else Logger.Log("selectedTab is null", LoggingTarget.Runtime, LogLevel.Error);

            selectedTab = tab;
            tab.Section.MoveToX(0, 400, Easing.OutQuint);
            tab.Select();
            Menu.SelectSection(tab.Section);
            tabSwitch?.Play();

            line.ResizeWidthTo(tab.TabContent.DrawWidth + 10, 400, Easing.OutQuint)
                .MoveToX(tab.Index * 60, 400, Easing.OutQuint);

            foreach (var child in Tabs.Children)
            {
                if (child != tab)
                    child.Deselect();
            }
        }
    }
}
