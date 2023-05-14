using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Input;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Settings.Sections;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Settings;

public partial class SettingsMenu : Container, IKeyBindingHandler<FluXisKeybind>
{
    [Resolved]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    public CategorySelector Selector { get; }
    public Container<SettingsSection> SectionContent { get; }

    private bool visible;
    private readonly SettingsContent content;

    public SettingsMenu()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        Add(new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Black,
            Alpha = .25f
        });

        Add(content = new SettingsContent
        {
            Width = 1200,
            Height = 600,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Scale = new Vector2(.95f),
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
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
                            Selector = new CategorySelector(this)
                        },
                        new Drawable[]
                        {
                            new BasicScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ScrollbarVisible = false,
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
        });

        createSection(new GeneralSection());
        createSection(new SkinSection());
        createSection(new GameplaySection());
        createSection(new InputSection());
        createSection(new AudioSection());
        createSection(new UserInterfaceSection());
        createSection(new GraphicsSection());
    }

    private void createSection(SettingsSection section)
    {
        Selector.AddTab(new SettingsCategoryTab(this, section));
        SectionContent.Add(section);
    }

    public void SelectSection(SettingsSection section)
    {
        SectionContent.Children.ForEach(s => s.Hide());
        section.Show();
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

        content.ScaleTo(.95f, 150, Easing.InQuint)
               .RotateTo(1, 150, Easing.InQuint);

        this.FadeOut(150);
    }

    public override void Show()
    {
        visible = true;
        cursorOverlay.ShowCursor = true;

        content.RotateTo(0)
               .ScaleTo(1f, 500, Easing.OutElastic);

        this.FadeIn(150);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Hide();
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
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

    private partial class SettingsContent : Container
    {
        protected override bool OnClick(ClickEvent e)
        {
            return true;
        }
    }

    public partial class CategorySelector : Container
    {
        private readonly CircularContainer line;
        private readonly Container<SettingsCategoryTab> tabs;

        private SettingsCategoryTab selectedTab;

        public SettingsMenu Menu { get; }

        public CategorySelector(SettingsMenu menu)
        {
            Menu = menu;

            Height = 50;
            AutoSizeAxes = Axes.X;
            Content.Origin = Content.Anchor = Anchor.TopCentre;

            AddInternal(tabs = new Container<SettingsCategoryTab>
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            });

            AddInternal(new Box
            {
                RelativeSizeAxes = Axes.X,
                Width = 4,
                Height = 3,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Colour = FluXisColors.Surface2
            });

            AddInternal(new Container
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
            });
        }

        protected override void Update()
        {
            for (var i = 0; i < tabs.Children.Count; i++)
            {
                var child = tabs.Children[i];

                if (i != 0)
                {
                    var prevChild = tabs.Children[i - 1];
                    child.X = prevChild.X + prevChild.DrawWidth + 10;
                }
                else
                    child.X = 0;
            }

            if (selectedTab != null)
                SelectTab(selectedTab);

            base.Update();
        }

        public void AddTab(SettingsCategoryTab tab)
        {
            tab.Index = tabs.Children.Count;
            tabs.Add(tab);

            if (selectedTab == null)
            {
                SelectTab(tab);
            }
        }

        public void SelectTab(SettingsCategoryTab tab = null)
        {
            tab ??= tabs.Children.First();

            // if still null, return
            if (tab == null)
                return;

            selectedTab = tab;
            tab.Select();
            Menu.SelectSection(tab.Section);

            line.ResizeWidthTo(tab.TabContent.DrawWidth + 10, 200, Easing.OutQuint)
                .MoveToX(tab.Index * 60, 200, Easing.OutQuint);

            foreach (var child in tabs.Children)
            {
                if (child != tab)
                    child.Deselect();
            }
        }
    }
}
