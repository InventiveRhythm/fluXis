using fluXis.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

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

        Child = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            CornerRadius = 10,
            Masking = true,
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(0.25f),
                Radius = 10,
                Offset = new Vector2(0, 1)
            },
            Children = new Drawable[]
            {
                dropdownContainer = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 0,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        },
                        dropdown = new SearchDropdown(Screen.Filters)
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 40,
                    Children = new Drawable[]
                    {
                        new SearchTextBox(this),
                        new DropdownIcon { Action = () => dropdownOpen.Value = !dropdownOpen.Value }
                    }
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
            dropdownContainer.ResizeHeightTo(0, 400, Easing.OutQuint);
    }

    protected override bool OnClick(ClickEvent e)
    {
        return true;
    }
}
