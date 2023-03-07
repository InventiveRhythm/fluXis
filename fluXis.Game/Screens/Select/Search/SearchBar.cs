using fluXis.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchBar : Container
{
    public SelectScreen Screen;

    private readonly Container dropdownContainer;
    private readonly SearchDropdown dropdown;

    private readonly BindableBool dropdownOpen = new();

    public SearchBar(SelectScreen screen)
    {
        Screen = screen;

        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;
        Width = .5f;
        Padding = new MarginPadding { Left = 20, Top = 10, Right = 10 };

        Children = new Drawable[]
        {
            new Container
            {
                CornerRadius = 5,
                Masking = true,
                RelativeSizeAxes = Axes.X,
                Height = 40,
                Children = new Drawable[]
                {
                    new SearchTextBox(this),
                    new DropdownIcon
                    {
                        Action = () => dropdownOpen.Value = !dropdownOpen.Value
                    }
                }
            },
            dropdownContainer = new Container
            {
                CornerRadius = 5,
                Masking = true,
                RelativeSizeAxes = Axes.X,
                Height = 0,
                Margin = new MarginPadding { Top = 50 },
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    dropdown = new SearchDropdown(Screen.Filters)
                }
            }
        };

        dropdownOpen.BindValueChanged(_ => updateDropdownStatus());
    }

    private void updateDropdownStatus()
    {
        if (dropdownOpen.Value)
            dropdownContainer.ResizeHeightTo(dropdown.DrawHeight, 400, Easing.OutQuint);
        else
            dropdownContainer.ResizeHeightTo(0, 400, Easing.InQuint);
    }

    protected override bool OnClick(ClickEvent e)
    {
        return true;
    }
}
