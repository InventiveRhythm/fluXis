using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menu;
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
    public Bindable<T> Bindable { get; set; }

    public IEnumerable<T> Items
    {
        get => items;
        set
        {
            items = value;

            if (menu != null) menu.Items = items;
        }
    }

    protected override bool IsDefault => Bindable.IsDefault;

    private SettingsDropdownMenu menu;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        Add(menu = new SettingsDropdownMenu
        {
            Items = Items,
            Current = Bindable
        });
    }

    protected override void Reset() => Bindable.SetDefault();

    private partial class SettingsDropdownMenu : Dropdown<T>
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Width = 400;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            Margin = new MarginPadding { Vertical = 5 };
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
                Foreground.Padding = new MarginPadding(5);
                CornerRadius = 5;
                BackgroundColour = FluXisColors.Background2;
                BackgroundColourHover = FluXisColors.Background3;

                Children = new Drawable[]
                {
                    label = new FluXisSpriteText
                    {
                        AlwaysPresent = true
                    },
                    new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.ChevronDown,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Size = new Vector2(14),
                        Margin = new MarginPadding { Right = 5 }
                    }
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                base.OnHover(e);
                return true;
            }
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

            public FluXisDropdownMenu()
            {
                MaskingContainer.CornerRadius = 5;
                BackgroundColour = FluXisColors.Background2;
                Margin = new MarginPadding { Top = 3 };
            }

            protected override void UpdateSize(Vector2 newSize)
            {
                if (Direction == Direction.Vertical)
                {
                    Width = newSize.X;
                    this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
                }
                else
                {
                    Height = newSize.Y;
                    this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
                }
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
                    BackgroundColour = Colour4.Transparent;
                    BackgroundColourHover = Colour4.White.Opacity(.2f);
                    BackgroundColourSelected = FluXisColors.Background6;
                    CornerRadius = 5;
                    Masking = true;
                }

                protected override Drawable CreateContent()
                {
                    return new FluXisSpriteText
                    {
                        FontSize = 16,
                        Margin = new MarginPadding { Vertical = 2, Horizontal = 5 },
                        Shadow = true
                    };
                }
            }
        }
    }
}
