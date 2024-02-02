using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Online.API.Models.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultTitle : Container
{
    private readonly RealmMap map;
    public APIUserShort User { get; init; }

    public ResultTitle(RealmMap map)
    {
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;

        InternalChild = new GridContainer
        {
            ColumnDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize),
                new Dimension(GridSizeMode.AutoSize),
                new Dimension(),
                new Dimension(GridSizeMode.AutoSize)
            },
            RowDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize)
            },
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Content = new[]
            {
                new[]
                {
                    new Container
                    {
                        Size = new Vector2(80),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding { Right = 10 },
                        CornerRadius = 10,
                        Masking = true,
                        Child = new MapCover(map.MapSet)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Y,
                        Width = 500,
                        Y = -2,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = map.Metadata.Title,
                                FontSize = 40,
                                RelativeSizeAxes = Axes.X,
                                Truncate = true
                            },
                            new FluXisSpriteText
                            {
                                Text = map.Metadata.Artist,
                                FontSize = 26,
                                Colour = FluXisColors.Text2,
                                Truncate = true,
                                RelativeSizeAxes = Axes.X,
                                Margin = new MarginPadding { Top = 33 }
                            },
                            new FluXisSpriteText
                            {
                                Text = $"[{map.Difficulty}] mapped by {map.Metadata.Mapper}",
                                FontSize = 22,
                                Colour = FluXisColors.Text2,
                                Truncate = true,
                                RelativeSizeAxes = Axes.X,
                                Margin = new MarginPadding { Top = 58 }
                            }
                        }
                    },
                    Empty(),
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                Margin = new MarginPadding { Right = 60 },
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                AutoSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = "Played by",
                                        FontSize = 16,
                                        Colour = FluXisColors.Text2,
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.BottomRight
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = User?.Username ?? "Guest",
                                        FontSize = 22,
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.TopRight
                                    }
                                }
                            },
                            new Container
                            {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                CornerRadius = 10,
                                Masking = true,
                                Child = new DrawableAvatar(User)
                                {
                                    Size = new Vector2(50)
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
