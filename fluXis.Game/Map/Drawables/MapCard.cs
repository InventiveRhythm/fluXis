using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menu;
using fluXis.Game.Map.Drawables.Online;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Screens.Select;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Map.Drawables;

public partial class MapCard : Container, IHasContextMenu
{
    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    public MenuItem[] ContextMenuItems
    {
        get
        {
            var list = new List<MenuItem>
            {
                new FluXisMenuItem("Select", MenuItemType.Highlighted, () => OnClickAction?.Invoke(MapSet))
            };

            if (downloaded)
            {
                list.Add(new FluXisMenuItem("Show in Song Select", FontAwesome.Solid.Eye, () =>
                {
                    if (localSet == null) return;

                    game.SelectMapSet(localSet);
                    game.MenuScreen.MakeCurrent();
                    game.MenuScreen.Push(new SelectScreen());
                }));
            }
            else
                list.Add(new FluXisMenuItem("Download", FontAwesome.Solid.Download, () => maps.DownloadMapSet(MapSet)));

            return list.ToArray();
        }
    }

    public APIMapSet MapSet { get; }
    public Action<APIMapSet> OnClickAction { get; set; }
    public bool ShowDownloadedState { get; set; } = true;

    private Box background;
    private Container content;

    private bool downloaded => maps.MapSets.Any(x => x.OnlineID == MapSet.Id);
    private bool downloading => maps.DownloadQueue.Any(x => x.Id == MapSet.Id);

    [CanBeNull]
    private RealmMapSet localSet => maps.MapSets.FirstOrDefault(x => x.OnlineID == MapSet.Id);

    public MapCard(APIMapSet mapSet)
    {
        MapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 430;
        Height = 100;
        CornerRadius = 20;
        Masking = true;
        BorderThickness = 2;

        if (MapSet == null)
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background3
                },
                new FluXisSpriteText
                {
                    Text = "Missing mapset data.",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
            return;
        }

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            content = new Container
            {
                Width = 430,
                RelativeSizeAxes = Axes.Y,
                Masking = true,
                CornerRadius = 20,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background3
                    },
                    new DelayedLoadUnloadWrapper(() => new DrawableOnlineBackground(MapSet))
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = .2f,
                                Colour = Colour4.Black.Opacity(.7f)
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.X,
                                Colour = ColourInfo.GradientHorizontal(Colour4.Black.Opacity(.7f), Colour4.Black.Opacity(.4f)),
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
                            new Dimension(GridSizeMode.Absolute, 100),
                            new Dimension()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new Container
                                {
                                    Size = new Vector2(100),
                                    CornerRadius = 20,
                                    Masking = true,
                                    Child = new DelayedLoadUnloadWrapper(() => new DrawableOnlineCover(MapSet))
                                    {
                                        RelativeSizeAxes = Axes.Both
                                    }
                                },
                                new FillFlowContainer
                                {
                                    Width = 320,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Spacing = new Vector2(-3),
                                    Padding = new MarginPadding { Horizontal = 10 },
                                    Children = new Drawable[]
                                    {
                                        new FluXisSpriteText
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Text = MapSet.Title,
                                            FontSize = 24,
                                            Shadow = true,
                                            Truncate = true
                                        },
                                        new FluXisSpriteText
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Text = $"by {MapSet.Artist}",
                                            FontSize = 16,
                                            Shadow = true,
                                            Truncate = true
                                        },
                                        new FluXisSpriteText
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Text = $"mapped by {MapSet.Creator?.GetName()}",
                                            FontSize = 16,
                                            Shadow = true,
                                            Truncate = true
                                        },
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 20,
                                            Margin = new MarginPadding { Top = 5 },
                                            Children = new Drawable[]
                                            {
                                                new CircularContainer
                                                {
                                                    AutoSizeAxes = Axes.X,
                                                    RelativeSizeAxes = Axes.Y,
                                                    Masking = true,
                                                    EdgeEffect = FluXisStyles.ShadowSmallNoOffset,
                                                    Children = new Drawable[]
                                                    {
                                                        new Box
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                            Colour = FluXisColors.GetStatusColor(MapSet.Status)
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Text = MapSet.Status switch
                                                            {
                                                                0 => "Unsubmitted",
                                                                1 => "Pending",
                                                                2 => "Impure",
                                                                3 => "Pure",
                                                                _ => "Unknown"
                                                            },
                                                            Colour = FluXisColors.TextDark,
                                                            FontSize = 16,
                                                            Margin = new MarginPadding { Horizontal = 8 }
                                                        }
                                                    }
                                                },
                                                new CircularContainer
                                                {
                                                    AutoSizeAxes = Axes.X,
                                                    RelativeSizeAxes = Axes.Y,
                                                    Masking = true,
                                                    EdgeEffect = FluXisStyles.ShadowSmallNoOffset,
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Children = new Drawable[]
                                                    {
                                                        new Box
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                            Colour = getKeymodeColor()
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            FontSize = 16,
                                                            Text = getKeymodeString(),
                                                            Margin = new MarginPadding { Horizontal = 8 }
                                                        }
                                                    }
                                                }
                                            }
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

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (ShowDownloadedState)
        {
            maps.MapSetAdded += mapsetsUpdated;
            maps.DownloadStarted += downloadStateChanged;
            maps.DownloadFinished += downloadStateChanged;
            updateState();
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapSetAdded -= mapsetsUpdated;
        maps.DownloadStarted -= downloadStateChanged;
        maps.DownloadFinished -= downloadStateChanged;
    }

    protected override bool OnClick(ClickEvent e)
    {
        OnClickAction?.Invoke(MapSet);
        return true;
    }

    private string getKeymodeString()
    {
        var lowest = MapSet.Maps.Min(x => x.KeyMode);
        var highest = MapSet.Maps.Max(x => x.KeyMode);

        return lowest == highest ? $"{lowest}K" : $"{lowest}-{highest}K";
    }

    private ColourInfo getKeymodeColor()
    {
        var lowest = MapSet.Maps.Min(x => x.KeyMode);
        var highest = MapSet.Maps.Max(x => x.KeyMode);

        return ColourInfo.GradientHorizontal(FluXisColors.GetKeyColor(lowest), FluXisColors.GetKeyColor(highest));
    }

    private void mapsetsUpdated(RealmMapSet set) => Schedule(updateState);
    private void downloadStateChanged(APIMapSet set) => Schedule(updateState);

    private void updateState()
    {
        bool shouldShow = downloading || downloaded;

        content.ResizeWidthTo(shouldShow ? 420 : 430, 400, Easing.OutQuint);

        if (downloading)
            background.Colour = Colour4.FromHex("#7BB1E8");
        else if (downloaded)
            background.Colour = Colour4.FromHex("#7BE87B");
        else
            background.Colour = FluXisColors.Background3;
    }
}
