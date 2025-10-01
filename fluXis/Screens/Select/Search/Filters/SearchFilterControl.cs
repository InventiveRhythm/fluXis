using System;
using System.Linq;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Select.Search.Filters;

public partial class SearchFilterControl<T> : CompositeDrawable
{
    private LocalisableString title { get; }
    private T[] values { get; }
    private Bindable<T> bind { get; }

    private const float item_height = 40;

    private Header header;
    private Container dropdown;

    public SearchFilterControl(LocalisableString title, T[] values, Bindable<T> bind)
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
                EdgeEffect = Styling.ShadowSmall,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background2
                    },
                    new FluXisScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = false,
                        Child = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            ChildrenEnumerable = values.Select(v => new Entry(v, GenerateItemText(v), GenerateItemIcon(v), () => bind.Value = v))
                        }
                    }
                }
            }
        };

        updateDropdown(false);
    }

    protected virtual IconUsage GenerateItemIcon(T item) => item.GetIcon();
    protected virtual LocalisableString GenerateItemText(T item) => item.GetLocalisableDescription();

    protected override void LoadComplete()
    {
        base.LoadComplete();
        bind.BindValueChanged(e => header.ValueText.Text = GenerateItemText(e.NewValue), true);
    }

    protected override bool OnClick(ClickEvent e) => true;

    private void updateDropdown(bool state)
    {
        if (state)
        {
            const float max_height = item_height * 8.5f;

            dropdown.ResizeHeightTo(Math.Min(item_height * values.Length, max_height), Styling.TRANSITION_MOVE, Easing.OutQuint)
                    .FadeEdgeEffectTo(Styling.SHADOW_OPACITY, Styling.TRANSITION_FADE);
        }
        else
        {
            dropdown.ResizeHeightTo(0, Styling.TRANSITION_MOVE, Easing.OutQuint)
                    .FadeEdgeEffectTo(0, Styling.TRANSITION_FADE);
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
            EdgeEffect = Styling.ShadowSmall;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2
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
                                        Colour = Theme.Background3
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

    protected partial class Entry : CompositeDrawable
    {
        private Action action { get; }
        private HoverLayer hover { get; }

        public Entry(T value, LocalisableString name, IconUsage icon, Action action)
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
                            Icon = icon,
                            Size = new Vector2(16),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Shear = new Vector2(.2f, 0)
                        },
                        new FluXisSpriteText
                        {
                            Text = name,
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
