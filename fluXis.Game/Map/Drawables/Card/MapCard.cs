using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Map.Drawables.Online;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game.Map.Drawables.Card;

public partial class MapCard : Container, IHasCustomTooltip<APIMapSet>, IHasContextMenu
{
    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    public MenuItem[] ContextMenuItems
    {
        get
        {
            var list = new List<MenuItem>
            {
                new FluXisMenuItem("Select", FontAwesome6.Solid.ArrowRight, MenuItemType.Highlighted, () => OnClickAction?.Invoke(MapSet))
            };

            if (downloaded)
                list.Add(new FluXisMenuItem("Show in Song Select", FontAwesome6.Solid.Eye, selectAndShow));
            else if (!downloading)
                list.Add(new FluXisMenuItem("Download", FontAwesome6.Solid.Download, download));

            list.Add(new FluXisMenuItem("Open in Web", FontAwesome6.Solid.EarthAmericas, () => host.OpenUrlExternally($"{api.Endpoint.WebsiteRootUrl}/set/{MapSet.ID}")));

            if (RequestDelete != null && canDelete)
                list.Add(new FluXisMenuItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, () => RequestDelete?.Invoke(MapSet.ID)));

            return list.ToArray();
        }
    }

    public APIMapSet TooltipContent => MapSet;

    public int CardWidth { get; init; } = 430;
    public APIMapSet MapSet { get; }
    public Action<APIMapSet> OnClickAction { get; set; }
    public bool ShowDownloadedState { get; set; } = true;

    [CanBeNull]
    public Action<long> RequestDelete { get; set; }

    private Box background;
    private Container content;
    private SectionedGradient gradient;

    private bool downloaded => maps.MapSets.Any(x => x.OnlineID == MapSet?.ID);
    private bool downloading => maps.DownloadQueue.Any(x => x == MapSet?.ID);

    private bool canDelete
    {
        get
        {
            var user = api.User.Value;

            if (user == null)
                return false;

            if (user.IsDeveloper() || user.CanModerate() || MapSet?.Creator.ID == user.ID)
                return true;

            return false;
        }
    }

    [CanBeNull]
    private RealmMapSet localSet => maps.MapSets.FirstOrDefault(x => x.OnlineID == MapSet?.ID);

    public MapCard(APIMapSet mapSet)
    {
        MapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = CardWidth;
        Height = 112;
        CornerRadius = 16;
        Masking = true;

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
                    Text = "MapSet was not found.",
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
                Width = CardWidth,
                RelativeSizeAxes = Axes.Y,
                Masking = true,
                CornerRadius = 16,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background3
                    },
                    new LoadWrapper<DrawableOnlineBackground>
                    {
                        RelativeSizeAxes = Axes.Both,
                        LoadContent = () => new DrawableOnlineBackground(MapSet),
                        OnComplete = d => d.FadeInFromZero(400)
                    },
                    gradient = new SectionedGradient
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2,
                        SplitPoint = .3f,
                        EndAlpha = .5f,
                        Alpha = .6f
                    },
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.Absolute, 112),
                            new Dimension()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new LoadWrapper<DrawableOnlineCover>
                                {
                                    Size = new Vector2(112),
                                    CornerRadius = 16,
                                    Masking = true,
                                    LoadContent = () => new DrawableOnlineCover(MapSet),
                                    OnComplete = d => d.FadeInFromZero(400)
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding(12),
                                    Children = new Drawable[]
                                    {
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(-3),
                                            Children = new Drawable[]
                                            {
                                                new GridContainer
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    AutoSizeAxes = Axes.Y,
                                                    RowDimensions = new Dimension[] { new(GridSizeMode.AutoSize) },
                                                    ColumnDimensions = new Dimension[]
                                                    {
                                                        new(),
                                                        new(GridSizeMode.AutoSize)
                                                    },
                                                    Content = new[]
                                                    {
                                                        new Drawable[]
                                                        {
                                                            new TruncatingText
                                                            {
                                                                RelativeSizeAxes = Axes.X,
                                                                Text = MapSet.Title,
                                                                WebFontSize = 18,
                                                                Shadow = true
                                                            },
                                                            new RoundedChip
                                                            {
                                                                Alpha = MapSet.Flags.HasFlag(MapSetFlag.Explicit) ? 1f : 0f,
                                                                Text = "EXPLICIT",
                                                                Anchor = Anchor.CentreRight,
                                                                Origin = Anchor.CentreRight,
                                                                BackgroundColour = Colour4.Black.Opacity(.5f),
                                                                TextColour = Colour4.White,
                                                                WebFontSize = 8,
                                                                Height = 16,
                                                                Margin = new MarginPadding { Left = 8 }
                                                            }
                                                        }
                                                    }
                                                },
                                                new TruncatingText
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    Text = $"by {MapSet.Artist}",
                                                    WebFontSize = 14,
                                                    Shadow = true
                                                },
                                                new TruncatingText
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    Text = $"mapped by {MapSet.Creator?.PreferredName}",
                                                    WebFontSize = 12,
                                                    Shadow = true,
                                                    Alpha = .8f
                                                }
                                            }
                                        },
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 20,
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft,
                                            Children = new Drawable[]
                                            {
                                                new RoundedChip
                                                {
                                                    Text = MapSet.Status switch
                                                    {
                                                        0 => "UNSUBMITTED",
                                                        1 => "PENDING",
                                                        2 => "IMPURE",
                                                        3 => "PURE",
                                                        _ => "UNKNOWN"
                                                    },
                                                    TextColour = Colour4.Black.Opacity(.75f),
                                                    BackgroundColour = FluXisColors.GetStatusColor(MapSet.Status),
                                                    EdgeEffect = FluXisStyles.ShadowSmallNoOffset,
                                                },
                                                new RoundedChip
                                                {
                                                    Text = getKeymodeString(),
                                                    TextColour = Colour4.Black.Opacity(.75f),
                                                    BackgroundColour = getKeymodeColor(),
                                                    EdgeEffect = FluXisStyles.ShadowSmallNoOffset,
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight
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

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        gradient.FadeTo(.5f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        gradient.FadeTo(.6f, 200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        OnClickAction?.Invoke(MapSet);
        return true;
    }

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        if (downloaded)
            selectAndShow();
        else if (!downloading)
            download();

        return true;
    }

    public ITooltip<APIMapSet> GetCustomTooltip() => new MapCardTooltip();

    private void selectAndShow()
    {
        if (localSet == null)
            return;

        game.ShowMap(localSet);
    }

    private void download() => maps.DownloadMapSet(MapSet);

    private string getKeymodeString()
    {
        var lowest = MapSet.Maps.Min(x => x.Mode);
        var highest = MapSet.Maps.Max(x => x.Mode);

        return lowest == highest ? $"{lowest}K" : $"{lowest}-{highest}K";
    }

    private ColourInfo getKeymodeColor()
    {
        var lowest = MapSet.Maps.Min(x => x.Mode);
        var highest = MapSet.Maps.Max(x => x.Mode);

        return ColourInfo.GradientHorizontal(FluXisColors.GetKeyColor(lowest), FluXisColors.GetKeyColor(highest));
    }

    private void mapsetsUpdated(RealmMapSet set) => Schedule(updateState);
    private void downloadStateChanged(long set) => Schedule(updateState);

    private void updateState()
    {
        if (content == null || background == null)
            return;

        bool shouldShow = downloading || downloaded;

        content.ResizeWidthTo(shouldShow ? CardWidth - 10 : CardWidth, 400, Easing.OutQuint);

        if (downloading)
            background.Colour = FluXisColors.DownloadQueued;
        else if (downloaded)
            background.Colour = FluXisColors.DownloadFinished;
        else
            background.Colour = FluXisColors.Background3;
    }
}
