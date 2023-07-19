using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Menu;
using fluXis.Game.Graphics.Scroll;
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
    public Bindable<T> Bindable { get; set; }
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    public override bool IsDefault => Bindable.IsDefault;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        Add(new SettingsDropdownMenu
        {
            Items = Items,
            Current = Bindable
        });
    }

    public override void Reset() => Bindable.SetDefault();

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
                BackgroundColour = Colour4.Black.Opacity(.2f);
                BackgroundColourHover = Colour4.White.Opacity(.2f);

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

            public FluXisDropdownMenu()
            {
                MaskingContainer.CornerRadius = 5;
                BackgroundColour = Colour4.Black.Opacity(.2f);
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
            }

            protected override void AnimateOpen()
            {
                this.FadeIn(200);
            }

            private partial class DrawableFluXisDropdownMenuItem : DrawableDropdownMenuItem
            {
                public DrawableFluXisDropdownMenuItem(MenuItem item)
                    : base(item)
                {
                    BackgroundColour = Colour4.Transparent;
                    BackgroundColourHover = Colour4.White.Opacity(.2f);
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
