using System;
using System.IO;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Import;
using fluXis.Game.Map.Drawables.Online;
using fluXis.Game.Online.API.Maps;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Types.Loading;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game.Map.Drawables;

public partial class MapCard : Container
{
    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private ImportManager importManager { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    public APIMapSet MapSet { get; }

    private bool downloading = false;

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
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            new DrawableOnlineBackground(MapSet)
            {
                RelativeSizeAxes = Axes.Both
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(10),
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new DrawableOnlineCover(MapSet)
                    {
                        Size = new Vector2(80),
                        CornerRadius = 10,
                        Masking = true
                    },
                    new FillFlowContainer
                    {
                        Width = 320,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(-3),
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
                                        EdgeEffect = new EdgeEffectParameters
                                        {
                                            Type = EdgeEffectType.Shadow,
                                            Colour = Colour4.Black.Opacity(0.25f),
                                            Radius = 5
                                        },
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
                                        EdgeEffect = new EdgeEffectParameters
                                        {
                                            Type = EdgeEffectType.Shadow,
                                            Colour = Colour4.Black.Opacity(0.25f),
                                            Radius = 5
                                        },
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
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (MapSet == null)
            return true;

        if (mapStore.MapSets.Any(x => x.OnlineID == MapSet.Id))
        {
            notifications.SendText("Mapset already downloaded.");
            return true;
        }

        if (downloading)
            return true;

        var notification = new LoadingNotificationData
        {
            TextLoading = "Downloading mapset...",
            TextSuccess = $"Downloaded {MapSet.Title} - {MapSet.Artist}.",
            TextFailure = "Failed to download mapset."
        };

        var req = fluxel.CreateAPIRequest($"/mapset/{MapSet.Id}/download");
        req.DownloadProgress += (current, total) => notification.Progress = (float)current / total;
        req.Started += () => Logger.Log($"Downloading mapset: {MapSet.Title} - {MapSet.Artist}", LoggingTarget.Network);
        req.Failed += exception =>
        {
            Logger.Log($"Failed to download mapset: {exception.Message}", LoggingTarget.Network);
            notification.State = LoadingState.Failed;
        };
        req.Finished += () =>
        {
            notification.Progress = 1;

            try
            {
                Logger.Log($"Finished downloading mapset: {MapSet.Title} - {MapSet.Artist}", LoggingTarget.Network);
                var data = req.GetResponseData();

                if (data == null)
                {
                    notification.State = LoadingState.Failed;
                    return;
                }

                // write data to file
                var path = storage.GetFullPath($"download/{MapSet.Id}.zip");
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (File.Exists(path))
                    File.Delete(path);

                File.WriteAllBytes(path, data);

                // import
                new FluXisImport
                {
                    MapStore = mapStore,
                    Storage = storage,
                    Notifications = notifications,
                    Realm = realm,
                    Notification = notification
                }.Import(path);
            }
            catch (Exception ex)
            {
                notification.State = LoadingState.Failed;
                Logger.Log($"Failed to import mapset: {ex.Message}", LoggingTarget.Network);
            }
        };

        downloading = true;
        req.PerformAsync();

        notifications.Add(notification);

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
}
