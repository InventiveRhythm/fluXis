using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Map;
using fluXis.Game.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game.Screens.Select.List;

public partial class MapSetHeader : Container, IHasContextMenu
{
    public MenuItem[] ContextMenuItems
    {
        get
        {
            List<MenuItem> items = new();

            if (!Equals(maps.CurrentMapSet, mapset))
                items.Add(new FluXisMenuItem("Select", FontAwesome6.Solid.ArrowRight, MenuItemType.Highlighted, () => maps.Select(mapset.LowestDifficulty, true)));

            items.Add(new FluXisMenuItem("Export", FontAwesome6.Solid.BoxOpen, MenuItemType.Normal, () => parent.ExportAction?.Invoke(mapset)) { Enabled = () => !mapset.Managed });
            items.Add(new FluXisMenuItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, () => parent.DeleteAction?.Invoke(mapset)));

            if (FluXisGameBase.IsDebug)
                items.Add(new FluXisMenuItem("Copy ID", FontAwesome6.Solid.Copy, MenuItemType.Normal, () => clipboard?.SetText(mapset.ID.ToString())));

            return items.ToArray();
        }
    }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    private readonly MapListEntry parent;
    private readonly RealmMapSet mapset;

    private float contentWrapperPadding
    {
        get => contentWrapper.Padding.Right;
        set => contentWrapper.Padding = new MarginPadding { Right = arrowContainer.Width = value };
    }

    private Box colorBrighten; // what do you call the opposite of dim?
    private Container contentWrapper;
    private DelayedLoadWrapper backgroundWrapper;
    private MapBackground background;
    private HeaderDim gradientBlack;
    private HeaderDim gradientColor;
    private DelayedLoadWrapper contentLoader;
    private FillFlowContainer diffsContainer;
    private Outline outlineBrighten;
    private Container arrowContainer;

    public MapSetHeader(MapListEntry parent, RealmMapSet mapset)
    {
        this.parent = parent;
        this.mapset = mapset;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        var color = mapset.Metadata.Color;

        RelativeSizeAxes = Axes.X;
        Height = 80;
        CornerRadius = 10;
        Masking = true;
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = color
            },
            colorBrighten = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.White.Opacity(.5f),
                Alpha = 0
            },
            contentWrapper = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Right = 10 },
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 10,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        backgroundWrapper = new DelayedLoadUnloadWrapper(() => background = new MapBackground(mapset.Maps[0], true)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }, 100, 200)
                        {
                            RelativeSizeAxes = Axes.Both
                        },
                        gradientBlack = new HeaderDim { Colour = Colour4.Black },
                        gradientColor = new HeaderDim { Colour = color },
                        contentLoader = new DelayedLoadUnloadWrapper(getContent, 100, 200)
                        {
                            RelativeSizeAxes = Axes.Both
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = .5f,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Padding = new MarginPadding { Right = -1 }, // to fix the 1px seam
                            Children = new Drawable[]
                            {
                                new Outline
                                {
                                    BorderColour = ColourInfo.GradientHorizontal(color.Opacity(0), color),
                                },
                                outlineBrighten = new Outline
                                {
                                    BorderColour = ColourInfo.GradientHorizontal(Colour4.White.Opacity(0), Colour4.White.Opacity(.5f)),
                                    Alpha = 0
                                }
                            }
                        }
                    }
                }
            },
            arrowContainer = new Container
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Child = new SpriteIcon
                {
                    X = -2,
                    Icon = FontAwesome6.Solid.ChevronLeft,
                    Size = new Vector2(16),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = color
                }
            }
        };

        backgroundWrapper.DelayedLoadComplete += background => background.FadeInFromZero(200);
        contentLoader.DelayedLoadComplete += content => content.FadeInFromZero(200);
    }

    private Drawable getContent()
    {
        var content = new Container
        {
            Padding = new MarginPadding { Right = 17 },
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 80),
                        new Dimension(GridSizeMode.Absolute, 10),
                        new Dimension(),
                        new Dimension(GridSizeMode.Absolute, 10),
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            new LoadWrapper<MapCover>
                            {
                                RelativeSizeAxes = Axes.Both,
                                CornerRadius = 10,
                                Masking = true,
                                LoadContent = () => new MapCover(mapset)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                },
                                OnComplete = cover => cover.FadeInFromZero(200)
                            },
                            Empty(),
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Spacing = new Vector2(0, -5),
                                Children = new Drawable[]
                                {
                                    new TruncatingText
                                    {
                                        WebFontSize = 20,
                                        Text = mapset.Metadata.Title,
                                        RelativeSizeAxes = Axes.X,
                                        Shadow = true
                                    },
                                    new TruncatingText
                                    {
                                        WebFontSize = 14,
                                        Text = mapset.Metadata.Artist,
                                        RelativeSizeAxes = Axes.X,
                                        Shadow = true
                                    }
                                }
                            },
                            Empty(),
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(6),
                                Children = new Drawable[]
                                {
                                    new StatusTag(mapset)
                                    {
                                        Anchor = Anchor.TopRight,
                                        Origin = Anchor.TopRight
                                    },
                                    diffsContainer = new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.X,
                                        Height = 20,
                                        Anchor = Anchor.TopRight,
                                        Origin = Anchor.TopRight,
                                        Direction = FillDirection.Horizontal,
                                        Spacing = new Vector2(2, 0)
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var groups = mapset.Maps.GroupBy(x => x.KeyCount).ToList();

        if (groups.Count > 1)
        {
            groups = groups.OrderBy(x => x.Key).ToList();

            foreach (var group in groups)
            {
                diffsContainer.Add(new CircularContainer
                {
                    Width = 24,
                    Height = 18,
                    Masking = true,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Margin = new MarginPadding { Left = 4, Right = 2 },
                    EdgeEffect = FluXisStyles.ShadowSmall,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.GetKeyColor(group.Key)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"{group.Key}K",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Colour = Colour4.Black,
                            Alpha = .75f,
                            FontSize = 14
                        }
                    }
                });

                var sorted = group.OrderBy(x => x.Filters.NotesPerSecond).ToList();

                foreach (var map in sorted)
                {
                    diffsContainer.Add(new TicTac(20)
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Colour = FluXisColors.GetDifficultyColor(map.Filters.NotesPerSecond),
                        EdgeEffect = FluXisStyles.ShadowSmall
                    });
                }
            }
        }
        else
        {
            var sorted = mapset.Maps.OrderBy(x => x.Filters.NotesPerSecond).ToList();

            foreach (var map in sorted)
            {
                diffsContainer.Add(new TicTac(20)
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Colour = FluXisColors.GetDifficultyColor(map.Filters.NotesPerSecond),
                    EdgeEffect = FluXisStyles.ShadowSmall
                });
            }
        }

        return content;
    }

    protected override bool OnHover(HoverEvent e)
    {
        gradientBlack.Show();
        gradientColor.Show();
        samples.Hover();
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        gradientBlack.Hide();
        gradientColor.Hide();
    }

    public override void Show()
    {
        this.TransformTo(nameof(contentWrapperPadding), 27f, 400, Easing.OutQuint);
        colorBrighten.FadeIn(200);
        outlineBrighten.FadeIn(200);
    }

    public override void Hide()
    {
        this.TransformTo(nameof(contentWrapperPadding), 10f, 400, Easing.OutQuint);
        colorBrighten.FadeOut(200);
        outlineBrighten.FadeOut(200);
    }

    private partial class Outline : Container
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Masking = true;
            CornerRadius = 10;
            BorderThickness = 3;

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Transparent
            };
        }
    }

    private partial class HeaderDim : Container
    {
        private const float dim_start = .4f;
        private const float dim_end = .1f;
        private const float hover_multiplier = .5f;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = .2f,
                    Colour = Colour4.White.Opacity(dim_start)
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Width = .8f,
                    Colour = ColourInfo.GradientHorizontal(Colour4.White.Opacity(dim_start), Colour4.White.Opacity(dim_end))
                }
            };
        }

        public override void Show() => this.FadeTo(hover_multiplier, 50);
        public override void Hide() => this.FadeIn(200);
    }
}
