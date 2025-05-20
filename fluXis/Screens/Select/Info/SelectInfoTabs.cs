using System;
using System.Collections.Generic;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Select.Info.Tabs;
using fluXis.Screens.Select.Info.Tabs.Scores;
using fluXis.Screens.Select.Info.Tabs.Settings;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Info;

public partial class SelectInfoTabs : CompositeDrawable
{
    public ScoreListTab Scores { get; private set; }

    private Bindable<SelectInfoTab> current;

    private Box backdrop;
    private FillFlowContainer tabFlow;

    private Container<SelectInfoTab> content;
    private Container headers;

    private Dictionary<SelectInfoTab, Drawable> headerMap;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding { Top = 12 };

        current = new Bindable<SelectInfoTab>();
        headerMap = new Dictionary<SelectInfoTab, Drawable>();

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = new Dimension[]
            {
                new(GridSizeMode.Absolute, 48),
                new()
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            backdrop = new Box
                            {
                                Height = 0.5f,
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background1,
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                                Alpha = 0
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                CornerRadius = 12,
                                Masking = true,
                                EdgeEffect = FluXisStyles.ShadowMedium,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = FluXisColors.Background2
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Right = 20 },
                                Children = new Drawable[]
                                {
                                    tabFlow = new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Padding = new MarginPadding { Horizontal = 16 },
                                        Direction = FillDirection.Horizontal,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Spacing = new Vector2(16)
                                    },
                                    headers = new Container
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight
                                    }
                                }
                            }
                        }
                    }
                },
                new Drawable[]
                {
                    content = new Container<SelectInfoTab>
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Right = 20 }
                    }
                }
            }
        };

        addTab(Scores = new ScoreListTab());
        addTab(new MapSettingsTab());
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        current.BindValueChanged(v =>
        {
            foreach (var tab in content)
            {
                var head = headerMap[tab];
                var alpha = tab == v.NewValue ? 1 : 0;
                tab.Alpha = head.Alpha = alpha;
            }

            backdrop.Alpha = v.NewValue.ShowBackdrop ? 1 : 0;
        }, true);
    }

    private void addTab(SelectInfoTab tab)
    {
        if (current.Value is null)
            current.Value = tab;

        tabFlow.Add(new TabItem(tab, current));

        var head = tab.CreateHeader();
        headers.Add(head);
        headerMap[tab] = head;

        content.Add(tab);
    }

    private partial class TabItem : FillFlowContainer
    {
        private Action action { get; }

        public TabItem(SelectInfoTab tab, Bindable<SelectInfoTab> current)
        {
            AutoSizeAxes = Axes.Both;
            Direction = FillDirection.Horizontal;
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            Spacing = new Vector2(6);

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteIcon
                {
                    Size = new Vector2(16),
                    Icon = tab.Icon,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                new FluXisSpriteText
                {
                    Text = tab.Title,
                    WebFontSize = 16,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                }
            };

            action = () => current.Value = tab;
            current.BindValueChanged(v => this.FadeTo(v.NewValue == tab ? 1f : .6f, 100), true);
        }

        protected override bool OnClick(ClickEvent e)
        {
            action?.Invoke();
            return true;
        }
    }
}
