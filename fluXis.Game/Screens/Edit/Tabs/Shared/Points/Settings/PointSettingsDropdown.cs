using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsDropdown<T> : Container, IHasTooltip
{
    public string Text { get; init; }
    public LocalisableString TooltipText { get; init; } = string.Empty;
    public T CurrentValue { get; init; }
    public List<T> Items { get; init; }
    public Action<T> OnValueChanged { get; set; }

    public Bindable<T> Bindable { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        Bindable ??= new Bindable<T>
        {
            Default = CurrentValue,
            Value = CurrentValue
        };

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = Text,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 16
            },
            new CustomDropdown
            {
                Width = 210,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Items = Items,
                Current = Bindable
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Bindable.BindValueChanged(valueChanged);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        Bindable.ValueChanged -= valueChanged;
    }

    private void valueChanged(ValueChangedEvent<T> e) => OnValueChanged?.Invoke(e.NewValue);

    private partial class CustomDropdown : FluXisDropdown<T>
    {
        protected override DropdownHeader CreateHeader() => new CustomHeader();
        protected override DropdownMenu CreateMenu() => new CustomMenu();

        private partial class CustomHeader : FluXisDropdownHeader
        {
            public CustomHeader()
            {
                BackgroundColour = FluXisColors.Background3;
                Height = 32;
                CornerRadius = 5;
                Foreground.Padding = new MarginPadding { Horizontal = 10 };
            }

            protected override DropdownSearchBar CreateSearchBar() => new CustomSearch();

            private partial class CustomSearch : FluXisDropdownSearchBar
            {
                protected override int SidePadding => 10;
            }
        }

        private partial class CustomMenu : FluXisDropdownMenu
        {
            public CustomMenu()
            {
                BackgroundColour = FluXisColors.Background3;
                MaskingContainer.CornerRadius = 5;
            }

            protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item)
                => new CustomMenuItem(item);

            private partial class CustomMenuItem : DrawableFluXisDropdownMenuItem
            {
                public CustomMenuItem(MenuItem item)
                    : base(item)
                {
                    BackgroundColour = FluXisColors.Background3;
                    BackgroundColourSelected = FluXisColors.Background5;
                }
            }
        }
    }
}
