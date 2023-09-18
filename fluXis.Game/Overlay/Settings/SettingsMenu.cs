using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Input;
using fluXis.Game.Overlay.Settings.Sections;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Overlay.Settings;

public partial class SettingsMenu : Container, IKeyBindingHandler<FluXisKeybind>
{
    public CategorySelector Selector { get; private set; }
    private Container<SettingsSection> sectionContent { get; set; }

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
                    Alpha = .5f
                }
            },
            content = new ClickableContainer
            {
                Width = 1400,
                Height = 800,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(.95f),
                Rotation = 2,
                CornerRadius = 20,
                Masking = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Colour4.Black.Opacity(.25f),
                    Radius = 10
                },
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
                                    Child = sectionContent = new Container<SettingsSection>
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Padding = new MarginPadding { Horizontal = 50, Top = 20, Bottom = 50 }
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
        Selector.AddTab(new SettingsCategoryTab(this) { Section = section });
        sectionContent.Add(section);
    }

    private void selectSection(SettingsSection section)
    {
        sectionContent.Children.ForEach(s => s.FadeOut(200));
        section.FadeIn(200);

        int index = sectionContent.IndexOf(section);
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

    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;

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

        public FillFlowContainer<SettingsCategoryTab> Tabs;

        private SettingsCategoryTab selectedTab;
        private Sample tabSwitch;

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            tabSwitch = samples.Get(@"UI/change-tab");

            Height = 70;
            AutoSizeAxes = Axes.X;
            Content.Origin = Content.Anchor = Anchor.TopCentre;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Width = 5f,
                    Height = 3f,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Colour = FluXisColors.Background4
                },
                Tabs = new FillFlowContainer<SettingsCategoryTab>
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Margin = new MarginPadding { Top = 10 },
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(5, 0)
                }
            };
        }

        protected override void LoadComplete()
        {
            ScheduleAfterChildren(() => SelectTab());
        }

        public void AddTab(SettingsCategoryTab tab) => Tabs.Add(tab);

        public void SelectTab(SettingsCategoryTab tab = null)
        {
            tab ??= Tabs.Children[0];

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
            Menu.selectSection(tab.Section);
            tabSwitch?.Play();

            foreach (var child in Tabs.Children)
            {
                if (child != tab)
                    child.Deselect();
            }
        }
    }
}
