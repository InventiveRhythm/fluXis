using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.Gameplay.Results;

public partial class MultiResultsHeader : CompositeDrawable
{
    private RealmMap map { get; }

    public MultiResultsHeader(RealmMap map)
    {
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 180;
        Masking = true;
        CornerRadius = 20;

        var accent = Colour4.White;

        if (!string.IsNullOrEmpty(map.Metadata.ColorHex) && Colour4.TryParseHex(map.Metadata.ColorHex, out var col))
            accent = col;

        InternalChildren = new Drawable[]
        {
            new MapBackground(map)
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = .4f,
                Colour = Colour4.Black,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .2f
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        RelativePositionAxes = Axes.X,
                        Colour = ColourInfo.GradientHorizontal(Colour4.White, Colour4.White.Opacity(0)),
                        Width = .8f,
                        X = .2f
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = .4f,
                Colour = accent,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .2f
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        RelativePositionAxes = Axes.X,
                        Colour = ColourInfo.GradientHorizontal(Colour4.White, Colour4.White.Opacity(0)),
                        Width = .8f,
                        X = .2f
                    }
                }
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize),
                    new Dimension()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new Container
                        {
                            Size = new Vector2(180),
                            CornerRadius = 20,
                            Masking = true,
                            Child = new MapCover(map.MapSet)
                            {
                                RelativeSizeAxes = Axes.Both,
                                FillMode = FillMode.Fill,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            }
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding(20),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                new FluXisSpriteText
                                {
                                    Text = map.Metadata.Title,
                                    WebFontSize = 36,
                                    Shadow = true,
                                    Margin = new MarginPadding { Bottom = -10 }
                                },
                                new FluXisSpriteText
                                {
                                    Text = map.Metadata.Artist,
                                    WebFontSize = 24,
                                    Shadow = true
                                },
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(5),
                                    Children = new Drawable[]
                                    {
                                        new DifficultyChip
                                        {
                                            Size = new Vector2(80, 20),
                                            Rating = map.Rating,
                                            FontSize = 19,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Margin = new MarginPadding { Right = 5 }
                                        },
                                        new FluXisSpriteText
                                        {
                                            Text = map.Difficulty,
                                            WebFontSize = 20,
                                            Shadow = true,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft
                                        },
                                        new FluXisSpriteText
                                        {
                                            Text = $"mapped by {map.Metadata.Mapper}",
                                            WebFontSize = 16,
                                            Shadow = true,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
