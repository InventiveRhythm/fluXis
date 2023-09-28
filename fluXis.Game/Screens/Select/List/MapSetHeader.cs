using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Cover;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
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

            if (!Equals(parent.Screen.MapSet.Value, mapset))
                items.Add(new FluXisMenuItem("Select", MenuItemType.Highlighted, () => parent.Screen.MapSet.Value = mapset));

            items.Add(new FluXisMenuItem("Export", FontAwesome.Solid.BoxOpen, MenuItemType.Normal, () => parent.Screen.ExportMapSet(mapset)));
            items.Add(new FluXisMenuItem("Delete", FontAwesome.Solid.Times, MenuItemType.Dangerous, () => parent.Screen.OpenDeleteConfirm(mapset)));

            if (FluXisGameBase.IsDebug)
                items.Add(new FluXisMenuItem("Copy ID", FontAwesome.Solid.Copy, MenuItemType.Normal, () => clipboard?.SetText(mapset.ID.ToString())));

            return items.ToArray();
        }
    }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    private readonly MapListEntry parent;
    private readonly RealmMapSet mapset;
    private DelayedLoadWrapper backgroundWrapper;
    private MapBackground background;
    private DelayedLoadWrapper coverWrapper;
    private DelayedLoadWrapper textWrapper;

    private HeaderDim dim;
    private HeaderDim colorDim;
    private FillFlowContainer diffsContainer;

    private bool colorsLoaded;
    private bool colorsTaskRunning;

    public MapSetHeader(MapListEntry parent, RealmMapSet mapset)
    {
        this.parent = parent;
        this.mapset = mapset;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        var color = Colour4.Transparent;

        if (!string.IsNullOrEmpty(mapset.Metadata.ColorHex) && Colour4.TryParseHex(mapset.Metadata.ColorHex, out var c))
            color = c;

        colorsLoaded = color != Colour4.Transparent;

        RelativeSizeAxes = Axes.X;
        Height = 80;
        CornerRadius = 10;
        Masking = true;
        BorderColour = colorsLoaded ? ColourInfo.GradientVertical(color.Lighten(1), color) : Colour4.White;
        Children = new Drawable[]
        {
            backgroundWrapper = new DelayedLoadUnloadWrapper(() => background = new MapBackground
            {
                Map = mapset.Maps[0],
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Cropped = true
            }, 100, 200)
            {
                RelativeSizeAxes = Axes.Both
            },
            dim = new HeaderDim { Colour = Colour4.Black },
            colorDim = new HeaderDim { Colour = color },
            textWrapper = new DelayedLoadUnloadWrapper(getContent, 100, 200)
            {
                RelativeSizeAxes = Axes.Both
            }
        };

        backgroundWrapper.DelayedLoadComplete += background => background.FadeInFromZero(200);
        textWrapper.DelayedLoadComplete += text =>
        {
            text.FadeInFromZero(200);
            coverWrapper.DelayedLoadComplete += cover => cover.FadeInFromZero(200);
        };
    }

    private Drawable getContent()
    {
        var content = new Container
        {
            Padding = new MarginPadding(10),
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 60),
                        new Dimension(GridSizeMode.Absolute, 10),
                        new Dimension(),
                        new Dimension(GridSizeMode.Absolute, 10),
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            coverWrapper = new DelayedLoadUnloadWrapper(() => new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Masking = true,
                                CornerRadius = 10,
                                Child = new DrawableCover(mapset)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                },
                                EdgeEffect = new EdgeEffectParameters
                                {
                                    Type = EdgeEffectType.Shadow,
                                    Colour = Colour4.Black.Opacity(.2f),
                                    Radius = 5,
                                    Offset = new Vector2(0, 1)
                                }
                            }, 100, 200)
                            {
                                Size = new Vector2(60)
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
                                    new FluXisSpriteText
                                    {
                                        FontSize = 32,
                                        Text = mapset.Metadata.Title,
                                        RelativeSizeAxes = Axes.X,
                                        Truncate = true,
                                        Shadow = true
                                    },
                                    new FluXisSpriteText
                                    {
                                        FontSize = 24,
                                        Text = mapset.Metadata.Artist,
                                        RelativeSizeAxes = Axes.X,
                                        Truncate = true,
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
                                Spacing = new Vector2(0, 4),
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
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Colour = Colour4.Black.Opacity(.25f),
                        Radius = 5,
                        Offset = new Vector2(0, 2)
                    },
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
                        EdgeEffect = new EdgeEffectParameters
                        {
                            Type = EdgeEffectType.Shadow,
                            Colour = Colour4.Black.Opacity(.25f),
                            Radius = 5,
                            Offset = new Vector2(0, 2)
                        }
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
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Colour = Colour4.Black.Opacity(.25f),
                        Radius = 5,
                        Offset = new Vector2(0, 2)
                    }
                });
            }
        }

        return content;
    }

    protected override void Update()
    {
        base.Update();

        if (colorsLoaded || colorsTaskRunning || background == null) return;

        colorsTaskRunning = true;

        Task.Run(() =>
        {
            var color = background.GetColour();

            if (color == Colour4.Transparent)
                return;

            Logger.Log($"Color for {mapset.Metadata.Title} is {color}");

            realm.RunWrite(r =>
            {
                var set = r.Find<RealmMapSet>(mapset.ID);

                foreach (var realmMap in set.Maps)
                    realmMap.Metadata.ColorHex = color.ToHex();
            });

            colorDim.Colour = color;
        });
    }

    protected override bool OnHover(HoverEvent e)
    {
        dim.Show();
        colorDim.Show();
        samples.Hover();
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        dim.Hide();
        colorDim.Hide();
    }

    public override void Show()
    {
        BorderThickness = 5;
    }

    public override void Hide()
    {
        BorderThickness = 0;
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
