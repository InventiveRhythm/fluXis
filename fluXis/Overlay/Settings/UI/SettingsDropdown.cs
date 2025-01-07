using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.UserInterface.Menus;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Overlay.Settings.UI;

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
            m.Width = 400;
            m.Anchor = Anchor.CentreRight;
            m.Origin = Anchor.CentreRight;
            m.Items = Items;
            m.Current = Bindable;
        }));
    }

    protected virtual Dropdown<T> CreateMenu() => new CustomDropdown();

    protected override void Reset() => Bindable.SetDefault();

    protected partial class CustomDropdown : FluXisDropdown<T>
    {
        protected override DropdownMenu CreateMenu() => new CustomMenu();

        protected partial class CustomMenu : FluXisDropdownMenu
        {
            public CustomMenu()
            {
                MaxHeight = 280;
                ScrollbarVisible = true;
            }
        }
    }
}
