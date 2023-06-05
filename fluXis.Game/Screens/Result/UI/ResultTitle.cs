using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Cover;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultTitle : Container
{
    public ResultTitle(RealmMap map)
    {
        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;

        APIUserShort user = Fluxel.LoggedInUser;

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
                        Child = new DrawableCover(map.MapSet) { RelativeSizeAxes = Axes.Both }
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
                                        Text = user?.Username ?? "Guest",
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
                                Child = new DrawableAvatar(user)
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
