using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Screens.Edit.UI.Variable;

public partial class EditorVariableDropdown<T> : EditorVariableBase
{
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
        Bindable.BindValueChanged(valueChanged);
        base.LoadComplete();
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
                BackgroundColour = Theme.Background3;
                Height = 32;
                CornerRadius = 5;
                Foreground.Padding = new MarginPadding { Horizontal = 10 };
            }

            protected override DropdownSearchBar CreateSearchBar() => new CustomSearch(UpdateOpenState);

            private partial class CustomSearch : FluXisDropdownSearchBar
            {
                public CustomSearch(Action<bool> stateAction)
                    : base(stateAction)
                {
                }

                protected override int SidePadding => 10;
            }
        }

        private partial class CustomMenu : FluXisDropdownMenu
        {
            public CustomMenu()
            {
                BackgroundColour = Theme.Background3;
                MaskingContainer.CornerRadius = 5;
                MaxHeight = 280;
                ScrollbarVisible = true;
            }

            protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item)
                => new CustomMenuItem(item);

            private partial class CustomMenuItem : DrawableFluXisDropdownMenuItem
            {
                public CustomMenuItem(MenuItem item)
                    : base(item)
                {
                    BackgroundColour = Theme.Background3;
                    BackgroundColourSelected = Theme.Background5;
                }
            }
        }
    }
}
