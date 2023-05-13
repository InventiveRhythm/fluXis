using System;
using System.IO;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Screens.Edit.MenuBar;
using fluXis.Game.Screens.Edit.Tabs;
using fluXis.Game.Screens.Edit.TabSwitcher;
using fluXis.Game.Screens.Edit.Timeline;
using fluXis.Game.Utils;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit;

public partial class Editor : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override bool ShowToolbar => false;

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    public RealmMap Map;
    public MapInfo OriginalMapInfo;
    public MapInfo MapInfo;

    private Container tabs;
    private int currentTab;

    private EditorClock clock;
    private Bindable<Waveform> waveform;
    private EditorChangeHandler changeHandler;
    private EditorValues values;

    private DependencyContainer dependencies;

    public Editor(RealmMap realmMap = null, MapInfo map = null)
    {
        Map = realmMap ?? RealmMap.CreateNew();
        OriginalMapInfo = map ?? new MapInfo(new MapMetadata());
        MapInfo = OriginalMapInfo.Clone();
        MapInfo.KeyCount = Map.KeyCount;

        Conductor.ResetLoop();
        Conductor.PauseTrack(); // the editor clock will handle this
    }

    [BackgroundDependencyLoader]
    private void load(BackgroundStack backgrounds, AudioManager audioManager, Storage storage)
    {
        backgrounds.AddBackgroundFromMap(Map);

        dependencies.CacheAs(waveform = new Bindable<Waveform>());
        dependencies.CacheAs(changeHandler = new EditorChangeHandler());
        dependencies.CacheAs(values = new EditorValues());

        values.MapInfo = MapInfo;

        changeHandler.OnTimingPointAdded += () => Logger.Log("Timing point added");
        changeHandler.OnTimingPointRemoved += () => Logger.Log("Timing point removed");
        changeHandler.OnTimingPointChanged += () => Logger.Log("Timing point changed");

        clock = new EditorClock(MapInfo);
        clock.ChangeSource(loadMapTrack(audioManager.GetTrackStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("files")))));
        dependencies.CacheAs(clock);

        InternalChildren = new Drawable[]
        {
            clock,
            tabs = new Container
            {
                Padding = new MarginPadding(10) { Top = 50, Bottom = 60 },
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new SetupTab(this),
                    new ComposeTab(this),
                    new TimingTab(this)
                }
            },
            new EditorMenuBar
            {
                Items = new MenuItem[]
                {
                    new("File")
                    {
                        Items = new MenuItem[]
                        {
                            new("Save", Save),
                            new("Export", Export),
                            new("Exit", TryExit)
                        }
                    },
                    new("Edit")
                    {
                        Items = new MenuItem[]
                        {
                            new("Undo", SendWipNotification),
                            new("Redo", SendWipNotification),
                            new("Cut", SendWipNotification),
                            new("Copy", SendWipNotification),
                            new("Paste", SendWipNotification),
                            new("Delete", SendWipNotification),
                            new("Select all", SendWipNotification)
                        }
                    },
                    new("View")
                    {
                        Items = new MenuItem[]
                        {
                            new("Waveform opacity")
                            {
                                Items = new MenuItem[]
                                {
                                    new("0%", () => values.WaveformOpacity.Value = 0),
                                    new("25%", () => values.WaveformOpacity.Value = 0.25f),
                                    new("50%", () => values.WaveformOpacity.Value = 0.5f),
                                    new("75%", () => values.WaveformOpacity.Value = 0.75f),
                                    new("100%", () => values.WaveformOpacity.Value = 1)
                                }
                            }
                        }
                    },
                    new("Timing")
                    {
                        Items = new MenuItem[]
                        {
                            new("Set preview point to current time", () =>
                            {
                                MapInfo.Metadata.PreviewTime = (int)clock.CurrentTime;
                                Map.Metadata.PreviewTime = (int)clock.CurrentTime;
                            })
                        }
                    }
                }
            },
            new EditorTabSwitcher
            {
                Children = new EditorTabSwitcherButton[]
                {
                    new()
                    {
                        Text = "Setup",
                        Action = () => ChangeTab(0)
                    },
                    new()
                    {
                        Text = "Compose",
                        Action = () => ChangeTab(1)
                    },
                    new()
                    {
                        Text = "Timing",
                        Action = () => ChangeTab(2)
                    }
                }
            },
            new EditorBottomBar { Editor = this }
        };
    }

    private Track loadMapTrack(ITrackStore trackStore)
    {
        string path = Map.MapSet?.GetFile(Map.Metadata?.Audio)?.GetPath();

        Waveform w = null;

        if (!string.IsNullOrEmpty(path))
        {
            Stream s = trackStore.GetStream(path);

            if (s != null)
                w = new Waveform(s);
        }

        waveform.Value = w;
        return trackStore.Get(path) ?? trackStore.GetVirtual(10000);
    }

    protected override void LoadComplete()
    {
        ChangeTab(0);
    }

    public void SortEverything()
    {
        MapInfo.HitObjects.Sort((a, b) => a.Time.CompareTo(b.Time));
        MapInfo.TimingPoints.Sort((a, b) => a.Time.CompareTo(b.Time));
        MapInfo.ScrollVelocities.Sort((a, b) => a.Time.CompareTo(b.Time));
    }

    public void ChangeTab(int to)
    {
        currentTab = to;

        if (currentTab < 0)
            currentTab = 0;
        if (currentTab >= tabs.Count)
            currentTab = tabs.Count - 1;

        for (var i = 0; i < tabs.Children.Count; i++)
        {
            Drawable tab = tabs.Children[i];
            tab.FadeTo(i == currentTab ? 1 : 0);
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.ControlPressed)
        {
            switch (e.Key)
            {
                case Key.Number1:
                    ChangeTab(0);
                    return true;

                case Key.Number2:
                    ChangeTab(1);
                    return true;

                case Key.Number3:
                    ChangeTab(2);
                    return true;

                case Key.S:
                    Save();
                    return true;
            }
        }

        return false;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e)
    {
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(100);
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(100);
        clock.Stop();
        Conductor.Seek((float)clock.CurrentTime);
        return false;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    public void Save()
    {
        if (Map == null)
            return;

        if (MapInfo.HitObjects.Count == 0)
        {
            notifications.PostError("Map has no hit objects!");
            return;
        }

        if (MapInfo.TimingPoints.Count == 0)
        {
            notifications.PostError("Map has no timing points!");
            return;
        }

        MapInfo.Sort();

        // get map as json
        string json = JsonConvert.SerializeObject(MapInfo);
        string hash = MapUtils.GetHash(json);

        // write to file
        string path = storage.GetFullPath($"files/{PathUtils.HashToPath(hash)}");
        string dir = Path.GetDirectoryName(path);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, json);

        // update realm
        realm.RunWrite(r =>
        {
            var existingMap = r.All<RealmMap>().FirstOrDefault(m => m.ID == Map.ID);

            if (existingMap != null)
            {
                var set = existingMap.MapSet;
                set.SetStatus(-2);
                var prevFile = existingMap.MapSet.GetFileFromHash(existingMap.Hash);

                if (prevFile == null)
                {
                    notifications.PostError("Failed to find previous map file!");
                    return;
                }

                prevFile.Hash = hash;
                existingMap.Hash = hash;
                r.Remove(existingMap.Filters);
                existingMap.Filters = MapUtils.GetMapFilters(MapInfo, new MapEvents());

                r.Remove(existingMap.Metadata);
                existingMap.Difficulty = MapInfo.Metadata.Difficulty;
                existingMap.Metadata = new RealmMapMetadata
                {
                    Title = MapInfo.Metadata.Title,
                    Artist = MapInfo.Metadata.Artist,
                    Mapper = MapInfo.Metadata.Mapper,
                    Source = MapInfo.Metadata.Source,
                    Tags = MapInfo.Metadata.Tags,
                    Background = MapInfo.BackgroundFile,
                    Audio = MapInfo.AudioFile,
                    PreviewTime = MapInfo.Metadata.PreviewTime,
                };

                mapStore.UpdateMapSet(Map.MapSet, set.Detach());
            }
            else
            {
                var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var set = Map.MapSet;
                set.Files.Add(new RealmFile
                {
                    Hash = hash,
                    Name = $"{time}.fsc",
                });

                Map.Hash = hash;
                Map.Filters = MapUtils.GetMapFilters(MapInfo, new MapEvents());
                Map.Difficulty = MapInfo.Metadata.Difficulty;
                Map.Metadata = new RealmMapMetadata
                {
                    Title = MapInfo.Metadata.Title,
                    Artist = MapInfo.Metadata.Artist,
                    Mapper = MapInfo.Metadata.Mapper,
                    Source = MapInfo.Metadata.Source,
                    Tags = MapInfo.Metadata.Tags,
                    Background = MapInfo.BackgroundFile,
                    Audio = MapInfo.AudioFile,
                    PreviewTime = MapInfo.Metadata.PreviewTime,
                };

                r.Add(set);
                mapStore.AddMapSet(set.Detach());
            }
        });

        notifications.Post("Saved!");
    }

    public void Export() => SendWipNotification();
    public void TryExit() => this.Exit(); // TODO: unsaved changes check
    public void SendWipNotification() => notifications.Post("This is still in development\nCome back later!");
}
