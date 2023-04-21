using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
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
                new Drawable[]
                {
                    new Container
                    {
                        AutoSizeAxes = Axes.Y,
                        Width = 500,
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Text = map.Metadata.Title,
                                Font = FluXisFont.Default(40),
                                RelativeSizeAxes = Axes.X,
                                Truncate = true
                            },
                            new SpriteText
                            {
                                Text = map.Metadata.Artist,
                                Font = FluXisFont.Default(26),
                                Colour = FluXisColors.Text2,
                                Truncate = true,
                                RelativeSizeAxes = Axes.X,
                                Margin = new MarginPadding { Top = 35 }
                            },
                            new SpriteText
                            {
                                Text = $"[{map.Difficulty}] mapped by {map.Metadata.Mapper}",
                                Font = FluXisFont.Default(22),
                                Colour = FluXisColors.Text2,
                                Truncate = true,
                                RelativeSizeAxes = Axes.X,
                                Margin = new MarginPadding { Top = 60 }
                            }
                        }
                    },
                    new Container(),
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
                                    new SpriteText
                                    {
                                        Text = "Played by",
                                        Font = FluXisFont.Default(16),
                                        Colour = FluXisColors.Text2,
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.BottomRight
                                    },
                                    new SpriteText
                                    {
                                        Text = user?.Username ?? "Guest",
                                        Font = FluXisFont.Default(22),
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
