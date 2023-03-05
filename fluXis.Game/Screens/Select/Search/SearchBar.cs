using fluXis.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchBar : Container
{
    public SelectScreen Screen;

    private readonly Container dropdownContainer;

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
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                }
            }
        };

        dropdownOpen.BindValueChanged(_ => updateDropdownStatus());
    }

    private void updateDropdownStatus()
    {
        if (dropdownOpen.Value)
            dropdownContainer.ResizeHeightTo(400, 400, Easing.OutQuint);
        else
            dropdownContainer.ResizeHeightTo(0, 400, Easing.InQuint);
    }
}
