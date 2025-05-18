using System;
using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Localization;
using fluXis.Utils;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Select.Search;

public partial class SearchFilterControls : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.X;
        Width = .5f;
        Height = 40;
        Y = 90;

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding { Left = 24 },
            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(-.2f, 0),
                ColumnDimensions = new Dimension[]
                {
                    new(),
                    new(GridSizeMode.Absolute, 12),
                    new(),
                },
                Content = new[]
                {
                    new[]
                    {
                        new Control<MapUtils.SortingMode>(LocalizationStrings.SongSelect.SortBy, Enum.GetValues<MapUtils.SortingMode>(),
                            config.GetBindable<MapUtils.SortingMode>(FluXisSetting.SortingMode)),
                        Empty(),
                        new Control<MapUtils.GroupingMode>(LocalizationStrings.SongSelect.GroupBy, Enum.GetValues<MapUtils.GroupingMode>(),
                            config.GetBindable<MapUtils.GroupingMode>(FluXisSetting.GroupingMode)),
                    }
                }
            }
        };
    }

    public override void Show() => this.MoveToX(-100).MoveToX(-10, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    public override void Hide() => this.MoveToX(-100, FluXisScreen.MOVE_DURATION, Easing.OutQuint);

    private partial class Control<T> : CompositeDrawable
    {
        private LocalisableString title { get; }
        private T[] values { get; }
        private Bindable<T> bind { get; }

        private const float item_height = 40;

        private Header header;
        private Container dropdown;

        public Control(LocalisableString title, T[] values, Bindable<T> bind)
        {
            this.title = title;
            this.values = values;
            this.bind = bind;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;

            InternalChildren = new Drawable[]
            {
                header = new Header(title, updateDropdown),
                dropdown = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 0,
                    Margin = new MarginPadding { Top = 44 },
                    CornerRadius = 8,
                    Masking = true,
                    EdgeEffect = FluXisStyles.ShadowSmall,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            ChildrenEnumerable = values.Select(v => new Entry(v, () => bind.Value = v))
                        }
                    }
                }
            };

            updateDropdown(false);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            bind.BindValueChanged(e => header.ValueText.Text = e.NewValue.GetLocalisableDescription(), true);
        }

        protected override bool OnClick(ClickEvent e) => true;

        private void updateDropdown(bool state)
        {
            if (state)
            {
                dropdown.ResizeHeightTo(item_height * values.Length, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
                        .FadeEdgeEffectTo(FluXisStyles.SHADOW_OPACITY, FluXisScreen.FADE_DURATION);
            }
            else
            {
                dropdown.ResizeHeightTo(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
                        .FadeEdgeEffectTo(0, FluXisScreen.FADE_DURATION);
            }
        }

        private partial class Header : CompositeDrawable
        {
            public FluXisSpriteText ValueText { get; }

            public override bool AcceptsFocus => true;

            private Action<bool> focusAction { get; }

            public Header(LocalisableString text, Action<bool> focusAction)
            {
                this.focusAction = focusAction;

                RelativeSizeAxes = Axes.Both;
                CornerRadius = 8;
                Masking = true;
                EdgeEffect = FluXisStyles.ShadowSmall;

                InternalChildren = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new Dimension[]
                        {
                            new(GridSizeMode.AutoSize),
                            new()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new Container
                                {
                                    AutoSizeAxes = Axes.X,
                                    RelativeSizeAxes = Axes.Y,
                                    CornerRadius = 8,
                                    Masking = true,
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = FluXisColors.Background3
                                        },
                                        new TruncatingText
                                        {
                                            Text = text,
                                            WebFontSize = 14,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Margin = new MarginPadding(14),
                                            Shear = new Vector2(.2f, 0),
                                            MaxWidth = 100
                                        }
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding(12),
                                    Child = ValueText = new TruncatingText
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        WebFontSize = 14,
                                        Shear = new Vector2(.2f, 0),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft
                                    }
                                }
                            }
                        }
                    }
                };
            }

            protected override bool OnClick(ClickEvent e) => !HasFocus;
            protected override void OnFocus(FocusEvent e) => focusAction?.Invoke(true);
            protected override void OnFocusLost(FocusLostEvent e) => focusAction?.Invoke(false);
        }

        private partial class Entry : CompositeDrawable
        {
            private Action action { get; }

            private HoverLayer hover { get; }

            public Entry(T value, Action action)
            {
                this.action = action;

                RelativeSizeAxes = Axes.X;
                Height = item_height;

                InternalChildren = new Drawable[]
                {
                    hover = new HoverLayer(),
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Padding = new MarginPadding { Horizontal = 12 },
                        Spacing = new Vector2(4),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteIcon
                            {
                                Icon = value.GetIcon(),
                                Size = new Vector2(16),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Shear = new Vector2(.2f, 0)
                            },
                            new FluXisSpriteText
                            {
                                Text = value.GetLocalisableDescription(),
                                WebFontSize = 14,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Shear = new Vector2(.2f, 0)
                            }
                        }
                    }
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                hover.Show();
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                hover.Hide();
            }

            protected override bool OnClick(ClickEvent e)
            {
                action();
                return true;
            }
        }
    }
}
