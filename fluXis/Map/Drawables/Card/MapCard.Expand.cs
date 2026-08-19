using System;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Requests.MapSets.Favorite;
using fluXis.Online.Fluxel;
using Midori.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Map.Drawables.Card;

public partial class MapCard
{
    private partial class Expand : ExpandingContainer
    {
        [Resolved]
        private IAPIClient api { get; set; }

        [Resolved]
        private MapStore maps { get; set; }

        private const float padding = 12;

        private const float diff_height = 14;
        private const float diff_spacing = 8;

        public const float BUTTON_HEIGHT = 32;

        protected override double HoverDelay => 400;

        public float ContentHeight => padding * 2
                                      + diff_height * set.Maps.Count
                                      + diff_spacing * (set.Maps.Count - 1)
                                      + BUTTON_HEIGHT;

        private readonly APIMapSet set;

        private readonly Button favorite;
        private readonly Button download;

        public Expand(APIMapSet set, Func<bool> view)
        {
            this.set = set;

            RelativeSizeAxes = Axes.X;
            Height = CARD_HEIGHT;
            CornerRadius = CARD_RADIUS;
            Masking = true;
            EdgeEffect = Styling.ShadowMedium;

            InternalChildren =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Top = CARD_HEIGHT },
                    Children =
                    [
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding(padding),
                            Spacing = new Vector2(diff_spacing),
                            ChildrenEnumerable = set.Maps.OrderBy(x => x.Rating).Select(m => new GridContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = diff_height,
                                ColumnDimensions =
                                [
                                    new Dimension(GridSizeMode.Absolute, 32),
                                    new Dimension(GridSizeMode.Absolute, 8),
                                    new Dimension(),
                                    new Dimension(GridSizeMode.Absolute, 8),
                                    new Dimension(GridSizeMode.Absolute, 48)
                                ],
                                Content = new[]
                                {
                                    new[]
                                    {
                                        createChip($"{m.Mode}K", Theme.GetKeyCountColor(m.Mode)).With(x =>
                                        {
                                            x.AutoSizeAxes = Axes.None;
                                            x.RelativeSizeAxes = Axes.X;
                                            x.Height = 14;
                                        }),
                                        Empty(),
                                        new FluXisSpriteText
                                        {
                                            Text = m.Difficulty,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            WebFontSize = 10
                                        },
                                        Empty(),
                                        createChip(m.Rating.ToStringInvariant("0.00"), Theme.GetDifficultyColor(m.Rating)).With(x =>
                                        {
                                            x.AutoSizeAxes = Axes.None;
                                            x.RelativeSizeAxes = Axes.X;
                                            x.Height = 14;
                                        }),
                                    }
                                }
                            })
                        },

                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = BUTTON_HEIGHT,
                            ColumnDimensions =
                            [
                                new Dimension(),
                                new Dimension(),
                                new Dimension()
                            ],
                            Content = new[]
                            {
                                new[]
                                {
                                    new Button
                                    {
                                        Icon = Phosphor.Bold.ArrowSquareUpRight,
                                        Action = view
                                    },
                                    favorite = new Button
                                    {
                                        Icon = Phosphor.Bold.HeartStraight,
                                        Action = () =>
                                        {
                                            toggleFavorite();
                                            return true;
                                        }
                                    },
                                    download = new Button
                                    {
                                        Icon = Phosphor.Bold.ArrowDown,
                                        Action = () =>
                                        {
                                            maps.DownloadMapSet(set);
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    ]
                }
            ];
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            maps.MapSetAdded += setAdded;
            updateDownloadState();
            updateFavoriteState();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            maps.MapSetAdded -= setAdded;
        }

        private void toggleFavorite()
        {
            var req = new MapFavoriteUpdateRequest(set.ID, !(set.Favorite ?? false));
            req.Success += res => Schedule(() =>
            {
                set.Favorite = res.Data.Favorite;
                updateFavoriteState();
            });
            api.PerformRequestAsync(req);
        }

        private void setAdded(RealmMapSet set) => updateDownloadState();

        private void updateDownloadState()
        {
            var downloaded = maps.MapSets.Any(s => s.OnlineID == set.ID);

            if (downloaded)
            {
                download.Icon = Phosphor.Bold.CaretDoubleRight;
                download.Colour = Theme.Green.Lighten(1.2f);
                download.Action = () =>
                {
                    maps.Present(maps.MapSets.FirstOrDefault(s => s.OnlineID == set.ID));
                    return true;
                };
            }
        }

        private void updateFavoriteState()
        {
            favorite.Icon = (set.Favorite ?? false ? Phosphor.Fill : Phosphor.Bold).HeartStraight;
            favorite.Colour = set.Favorite ?? false ? Theme.Pink : Theme.Text;
        }
    }
}
