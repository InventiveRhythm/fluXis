using System.Linq;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.Drawables.Images;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Map.Drawables.Card;

public partial class MapCard
{
    private partial class Header : CompositeDrawable
    {
        public Header(APIMapSet set)
        {
            RelativeSizeAxes = Axes.X;
            Height = CARD_HEIGHT;
            CornerRadius = CARD_RADIUS;
            Masking = true;

            var color = Colour4.FromRGBA(set.Maps.First().Color);
            var hsv = color.ToHSV();
            var light = Colour4.FromHSV(hsv.X, .1f, 1f);

            InternalChildren =
            [
                new LoadWrapper<DrawableOnlineBackground>
                {
                    RelativeSizeAxes = Axes.Both,
                    LoadContent = () => new DrawableOnlineBackground(set),
                    OnComplete = d =>
                    {
                        d.FadeInFromZero(Styling.TRANSITION_FADE);
                    }
                },
                new VerticalSectionedGradient
                {
                    RelativeSizeAxes = Axes.Both,
                    StartAlpha = .75f,
                    EndAlpha = .25f,
                    Colour = Colour4.Black,
                    Alpha = .75f
                },
                new VerticalSectionedGradient
                {
                    RelativeSizeAxes = Axes.Both,
                    StartAlpha = .75f,
                    EndAlpha = .25f,
                    Colour = color
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(8),
                    Spacing = new Vector2(8),
                    Children =
                    [
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 88,
                            ColumnDimensions =
                            [
                                new Dimension(GridSizeMode.Absolute, 88),
                                new Dimension(GridSizeMode.Absolute, 8),
                                new Dimension()
                            ],
                            Content = new[]
                            {
                                new[]
                                {
                                    new LoadWrapper<DrawableOnlineCover>
                                    {
                                        Size = new Vector2(88),
                                        CornerRadius = CARD_RADIUS,
                                        Masking = true,
                                        LoadContent = () => new DrawableOnlineCover(set),
                                        OnComplete = d => d.FadeInFromZero(Styling.TRANSITION_FADE)
                                    },
                                    Empty(),
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Children =
                                        [
                                            new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.X,
                                                RelativeSizeAxes = Axes.Y,
                                                Direction = FillDirection.Horizontal,
                                                Spacing = new Vector2(8),
                                                Anchor = Anchor.TopRight,
                                                Origin = Anchor.TopRight,
                                                Children =
                                                [
                                                    createDarkChip("FEATURED ARTIST").With(x => x.Alpha = set.Flags.HasFlagFast(MapSetFlag.FeaturedArtist) ? 1 : 0),
                                                    createDarkChip("EXPLICIT").With(x => x.Alpha = set.Flags.HasFlagFast(MapSetFlag.Explicit) ? 1 : 0)
                                                ]
                                            },
                                            new FillFlowContainer
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Spacing = new Vector2(4),
                                                Padding = new MarginPadding { Bottom = 8 },
                                                Anchor = Anchor.BottomLeft,
                                                Origin = Anchor.BottomLeft,
                                                Children =
                                                [
                                                    new ForcedHeightText(true)
                                                    {
                                                        Text = set.LocalizedArtist,
                                                        RelativeSizeAxes = Axes.X,
                                                        WebFontSize = 14,
                                                        Height = 14,
                                                        Alpha = 0.8f,
                                                        Colour = light
                                                    },
                                                    new ForcedHeightText(true)
                                                    {
                                                        Text = set.LocalizedTitle,
                                                        RelativeSizeAxes = Axes.X,
                                                        WebFontSize = 18,
                                                        Height = 18,
                                                        Colour = light
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                }
                            }
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 16,
                            Children =
                            [
                                createChip(set.Status switch
                                {
                                    -1 => "BLACKLISTED",
                                    0 => "UNSUBMITTED",
                                    1 => "PENDING",
                                    2 => "IMPURE",
                                    3 => "PURE",
                                    _ => "UNKNOWN"
                                }, Theme.GetStatusColor(set.Status)),
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.X,
                                    RelativeSizeAxes = Axes.Y,
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(8),
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Children =
                                    [
                                        createChip(getRatingString(set), getRatingColor(set)),
                                        createChip(getModeString(set), getModeColor(set)),
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ];
        }

        private static RoundedChip createDarkChip(string text) => new()
        {
            Text = text,
            TextColour = Theme.Text,
            BackgroundColour = Colour4.Black.Opacity(0.5f),
            WebFontSize = 10,
            Height = 16
        };

        private string getModeString(APIMapSet set)
        {
            var lowest = set.Maps.Min(x => x.Mode);
            var highest = set.Maps.Max(x => x.Mode);

            return lowest == highest ? $"{lowest}K" : $"{lowest}-{highest}K";
        }

        private ColourInfo getModeColor(APIMapSet set)
        {
            var lowest = set.Maps.Min(x => x.Mode);
            var highest = set.Maps.Max(x => x.Mode);

            return ColourInfo.GradientHorizontal(Theme.GetKeyCountColor(lowest), Theme.GetKeyCountColor(highest));
        }

        private string getRatingString(APIMapSet set)
        {
            var lowest = set.Maps.MinBy(x => x.Rating);
            var highest = set.Maps.MaxBy(x => x.Rating);

            var lowestStr = $"{lowest.Rating:0.00}";
            var highestStr = $"{highest.Rating:0.00}";

            if (lowest.Rating <= 0)
                lowestStr = $"{set.Maps.Min(x => x.NotesPerSecond) / 2f:0.00}?";
            if (highest.Rating <= 0)
                highestStr = $"{set.Maps.Max(x => x.NotesPerSecond) / 2f:0.00}?";

            return lowestStr == highestStr ? lowestStr : $"{lowestStr} - {highestStr}";
        }

        private ColourInfo getRatingColor(APIMapSet set)
        {
            var lowest = set.Maps.MinBy(x => x.Rating);
            var highest = set.Maps.MaxBy(x => x.Rating);

            var lowestCol = Theme.GetDifficultyColor(lowest.Rating);
            var highestCol = Theme.GetDifficultyColor(highest.Rating);

            if (lowest.Rating <= 0)
                lowestCol = Theme.GetDifficultyColor(set.Maps.Min(x => x.NotesPerSecond) / 2f).Darken(.5f);
            if (highest.Rating <= 0)
                highestCol = Theme.GetDifficultyColor(set.Maps.Max(x => x.NotesPerSecond) / 2f).Darken(.5f);

            return ColourInfo.GradientHorizontal(lowestCol, highestCol);
        }
    }
}
