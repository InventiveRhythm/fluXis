using System;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchFilterControls : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.X;
        Width = .5f;
        Height = 40;
        Y = 90;

        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(12),
            Padding = new MarginPadding { Left = 24 },
            Shear = new Vector2(-.2f, 0),
            Children = new Drawable[]
            {
                new Control<MapUtils.SortingMode>("sort by", Enum.GetValues<MapUtils.SortingMode>(), config.GetBindable<MapUtils.SortingMode>(FluXisSetting.SortingMode)),
                new Control<MapUtils.GroupingMode>("group by", Enum.GetValues<MapUtils.GroupingMode>(), config.GetBindable<MapUtils.GroupingMode>(FluXisSetting.GroupingMode)),
            }
        };
    }

    public override void Show() => this.MoveToX(-100).MoveToX(-10, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    public override void Hide() => this.MoveToX(-100, FluXisScreen.MOVE_DURATION, Easing.OutQuint);

    private partial class Control<T> : CompositeDrawable
    {
        private string title { get; }
        private T[] values { get; }
        private Bindable<T> bind { get; }

        private const float item_height = 40;

        private Header header;
        private Container dropdown;

        public Control(string title, T[] values, Bindable<T> bind)
        {
            this.title = title;
            this.values = values;
            this.bind = bind;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.X;
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
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            bind.BindValueChanged(e => header.ValueText.Text = e.NewValue.GetDescription(), true);
        }

        protected override bool OnClick(ClickEvent e) => true;

        private void updateDropdown(bool state)
        {
            if (state)
                dropdown.ResizeHeightTo(item_height * values.Length, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
            else
                dropdown.ResizeHeightTo(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        }

        private partial class Header : CompositeDrawable
        {
            public FluXisSpriteText ValueText { get; }

            public override bool AcceptsFocus => true;

            private Action<bool> focusAction { get; }

            public Header(string text, Action<bool> focusAction)
            {
                this.focusAction = focusAction;

                AutoSizeAxes = Axes.X;
                RelativeSizeAxes = Axes.Y;
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
                        AutoSizeAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y,
                        ColumnDimensions = new Dimension[]
                        {
                            new(GridSizeMode.AutoSize),
                            new(GridSizeMode.Absolute, 220)
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
                                        new FluXisSpriteText
                                        {
                                            Text = text,
                                            WebFontSize = 14,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Margin = new MarginPadding(14),
                                            Shear = new Vector2(.2f, 0)
                                        }
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding(12),
                                    Child = ValueText = new TruncatingText
                                    {
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

            private Box hover { get; }

            public Entry(T value, Action action)
            {
                this.action = action;

                RelativeSizeAxes = Axes.X;
                Height = item_height;

                InternalChildren = new Drawable[]
                {
                    hover = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Padding = new MarginPadding { Horizontal = 12 },
                        Spacing = new Vector2(4),
                        Children = new Drawable[]
                        {
                            new SpriteIcon
                            {
                                Icon = value.GetIcon(),
                                Size = new Vector2(16),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Shear = new Vector2(.2f, 0)
                            },
                            new FluXisSpriteText
                            {
                                Text = value.GetDescription(),
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
                hover.FadeTo(.2f, 50);
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                hover.FadeOut(200);
            }

            protected override bool OnClick(ClickEvent e)
            {
                action();
                return true;
            }
        }
    }
}
