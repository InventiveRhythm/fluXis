using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsDropdown<T> : SettingsItem
{
    private IEnumerable<T> items = Enumerable.Empty<T>();
    private Bindable<T> bindable = new();

    public Bindable<T> Bindable
    {
        get => bindable;
        set
        {
            bindable = value;

            if (menu != null)
                menu.Current = bindable;
        }
    }

    public IEnumerable<T> Items
    {
        get => items;
        set
        {
            items = value;

            if (menu != null)
                menu.Items = items;
        }
    }

    protected override bool IsDefault => Bindable.IsDefault;

    private Dropdown<T> menu;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        Add(menu = CreateMenu().With(m =>
        {
            m.Items = Items;
            m.Current = Bindable;
        }));
    }

    protected virtual Dropdown<T> CreateMenu() => new SettingsDropdownMenu();

    protected override void Reset() => Bindable.SetDefault();

    protected partial class SettingsDropdownMenu : Dropdown<T>
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Width = 400;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
        }

        protected override DropdownHeader CreateHeader() => new FluXisDropdownHeader();
        protected override DropdownMenu CreateMenu() => new FluXisDropdownMenu();

        private partial class FluXisDropdownHeader : DropdownHeader
        {
            private readonly FluXisSpriteText label;

            protected override LocalisableString Label
            {
                get => label.Text;
                set => label.Text = value;
            }

            public FluXisDropdownHeader()
            {
                AutoSizeAxes = Axes.None;
                Height = 40;
                Foreground.Padding = new MarginPadding { Left = 14, Right = 10 };
                CornerRadius = 10;
                BackgroundColour = FluXisColors.Background2;
                BackgroundColourHover = FluXisColors.Background4;

                Children = new Drawable[]
                {
                    label = new FluXisSpriteText
                    {
                        WebFontSize = 16,
                        AlwaysPresent = true,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new SpriteIcon
                    {
                        Icon = FontAwesome6.Solid.ChevronDown,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Size = new Vector2(16),
                        Margin = new MarginPadding { Right = 5 }
                    }
                };
            }

            protected override DropdownSearchBar CreateSearchBar() => new SettingsDropdownSearchBar();

            protected override bool OnHover(HoverEvent e)
            {
                base.OnHover(e);
                return true;
            }
        }

        private partial class SettingsDropdownSearchBar : DropdownSearchBar
        {
            protected override TextBox CreateTextBox()
            {
                return new FluXisTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 40,
                    SidePadding = 14,
                    FontSize = FluXisSpriteText.GetWebFontSize(16),
                    PlaceholderText = "Search"
                };
            }

            protected override void PopIn() => this.FadeIn(200);
            protected override void PopOut() => this.FadeOut(200);
        }

        private partial class FluXisDropdownMenu : DropdownMenu
        {
            protected override Menu CreateSubMenu() => new FluXisMenu(Direction.Vertical);

            protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item) => new DrawableFluXisDropdownMenuItem(item);

            protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new FluXisScrollContainer(direction)
            {
                ScrollbarVisible = false
            };

            [Resolved]
            private UISamples samples { get; set; }

            private bool justLoaded = true;

            private float marginTop
            {
                get => Margin.Top;
                set => Margin = new MarginPadding { Top = value };
            }

            public FluXisDropdownMenu()
            {
                MaskingContainer.CornerRadius = 10;
                BackgroundColour = FluXisColors.Background2;
                Margin = new MarginPadding { Top = 8 };
            }

            protected override void UpdateSize(Vector2 newSize)
            {
                if (Direction != Direction.Vertical)
                    return;

                Width = newSize.X;
                this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
                this.TransformTo(nameof(marginTop), newSize.Y > 0 ? 8f : 0f, 300, Easing.OutQuint);
            }

            protected override void AnimateClose()
            {
                this.FadeOut(200);

                if (!justLoaded) // AnimateClose is called on load, so we don't want to play the sound then
                    samples.Dropdown(true);

                justLoaded = false;
            }

            protected override void AnimateOpen()
            {
                this.FadeIn(200);
                samples.Dropdown(false);
            }

            private partial class DrawableFluXisDropdownMenuItem : DrawableDropdownMenuItem
            {
                public DrawableFluXisDropdownMenuItem(MenuItem item)
                    : base(item)
                {
                    BackgroundColour = FluXisColors.Background2;
                    BackgroundColourHover = FluXisColors.Background4;
                    BackgroundColourSelected = FluXisColors.Background3;

                    Foreground.AutoSizeAxes = Axes.X;
                    Foreground.Height = 30;
                    Foreground.Padding = new MarginPadding { Horizontal = 14 };
                }

                public override void SetFlowDirection(Direction direction)
                {
                    RelativeSizeAxes = Axes.X;
                    AutoSizeAxes = Axes.None;
                    Height = 30;
                }

                protected override Drawable CreateContent()
                {
                    return new FluXisSpriteText
                    {
                        WebFontSize = 12,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    };
                }
            }
        }
    }
}
