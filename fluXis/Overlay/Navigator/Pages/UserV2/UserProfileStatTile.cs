using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.UserV2;

#nullable enable

public partial class UserProfileStatTile : CompositeDrawable
{
    private readonly LocalisableString title;
    private readonly string value;
    private readonly Drawable? icon;

    public UserProfileStatTile(LocalisableString title, string value, Drawable? icon = null)
    {
        this.title = title;
        this.value = value;
        this.icon = icon;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;
        CornerRadius = 6;
        Masking = true;

        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 10, Vertical = 6 },
                Child = createGrid(new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(-2),
                    Children =
                    [
                        new TruncatingText
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = value,
                            WebFontSize = 14
                        },
                        new TruncatingText
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = title,
                            WebFontSize = 10,
                            Alpha = .8f
                        }
                    ]
                })
            }
        ];

        GridContainer createGrid(Drawable flow)
        {
            var grid = new GridContainer { RelativeSizeAxes = Axes.Both };

            if (icon != null)
            {
                grid.ColumnDimensions = [new Dimension(GridSizeMode.Absolute, 36), new Dimension(GridSizeMode.Absolute, 8), new Dimension()];
                grid.Content = new[] { new[] { icon.With(x => x.Anchor = x.Origin = Anchor.Centre), Empty(), flow } };
            }
            else
            {
                grid.ColumnDimensions = [new Dimension()];
                grid.Content = new[] { new[] { flow } };
            }

            return grid;
        }
    }
}
