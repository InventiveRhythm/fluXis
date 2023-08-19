using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.Search;

public partial class SearchBar : Container
{
    public SearchFilters Filters { get; init; }

    private SearchDropdown dropdown;

    private readonly BindableBool dropdownOpen = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Width = .5f;
        Height = 60;
        X = -10;
        Y = 10;
        Padding = new MarginPadding { Left = -10 };

        InternalChildren = new Drawable[]
        {
            dropdown = new SearchDropdown { Filters = Filters },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 10,
                Masking = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Color4.Black.Opacity(0.25f),
                    Radius = 10,
                    Offset = new Vector2(0, 1)
                },
                Shear = new Vector2(-.2f, 0),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background1
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
                                    Colour = FluXisColors.Background2
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Left = 110, Right = 20 },
                                Shear = new Vector2(.2f, 0),
                                Child = new SearchTextBox { Search = Filters }
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
                                    Colour = FluXisColors.Background3
                                }
                            },
                            new SpriteIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Icon = FontAwesome.Solid.Search,
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

    private void updateDropdownStatus()
    {
        if (dropdownOpen.Value) dropdown.Show();
        else dropdown.Hide();
    }

    public override void Show() => this.MoveToX(-200).MoveToX(-10, 400, Easing.OutQuint);
    public override void Hide() => this.MoveToX(-200, 400, Easing.OutQuint);

    protected override bool OnClick(ClickEvent e) => true;
}
