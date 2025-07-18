using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Select.Search;

public partial class SearchBar : Container
{
    private SearchDropdown dropdown;

    private readonly BindableBool dropdownOpen = new();

    private SearchTextBox textBox;
    private IFocusManager focus;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Width = .5f;
        Height = 60;
        X = -10;
        Y = 20;
        Padding = new MarginPadding { Left = -10 };

        InternalChildren = new Drawable[]
        {
            dropdown = new SearchDropdown(),
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 10,
                Masking = true,
                EdgeEffect = Styling.ShadowMedium,
                Shear = new Vector2(-.2f, 0),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background1
                    },
                    new Container
                    {
                        Size = new Vector2(60),
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Shear = new Vector2(.2f, 0),
                        Child = new DropdownIcon { Action = dropdownOpen.Toggle }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Right = 60 },
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                CornerRadius = 10,
                                Masking = true,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Theme.Background2
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Left = 110, Right = 20 },
                                Shear = new Vector2(.2f, 0),
                                Child = textBox = new SearchTextBox()
                            }
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 90,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                CornerRadius = 10,
                                Masking = true,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Theme.Background3
                                }
                            },
                            new FluXisSpriteIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Icon = FontAwesome6.Solid.MagnifyingGlass,
                                Shear = new Vector2(.2f, 0),
                                Size = new Vector2(24),
                                Shadow = true,
                                Margin = new MarginPadding { Left = 10 }
                            }
                        }
                    }
                }
            }
        };

        dropdownOpen.BindValueChanged(_ => updateDropdownStatus());
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        focus = GetContainingFocusManager();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key != Key.Tab)
            return false;

        focus.ChangeFocus(textBox.HasFocus ? null : textBox);
        return true;
    }

    private void updateDropdownStatus()
    {
        if (dropdownOpen.Value) dropdown.Show();
        else dropdown.Hide();
    }

    public override void Show() => this.MoveToX(-100).MoveToX(-10, Styling.TRANSITION_MOVE, Easing.OutQuint);
    public override void Hide() => this.MoveToX(-100, Styling.TRANSITION_MOVE, Easing.OutQuint);

    protected override bool OnClick(ClickEvent e) => true;
}
