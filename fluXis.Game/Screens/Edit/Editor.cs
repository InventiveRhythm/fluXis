using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Graphics.UserInterface.Menu;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Types.Loading;
using fluXis.Game.Screens.Edit.MenuBar;
using fluXis.Game.Screens.Edit.Tabs;
using fluXis.Game.Screens.Edit.Tabs.Charting;
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
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit;

public partial class Editor : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override bool ShowToolbar => false;
    public override float BackgroundDim => 0.4f;
    public override bool AllowMusicControl => false;

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private AudioClock audioClock { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    private ITrackStore trackStore { get; set; }

    private EditorLoader loader { get; }

    public RealmMap Map;
    public EditorMapInfo MapInfo;

    private Container tabs;
    private int currentTab;

    private EditorMenuBar menuBar;
    private EditorTabSwitcher tabSwitcher;
    private EditorBottomBar bottomBar;

    private EditorClock clock;
    private Bindable<Waveform> waveform;
    private EditorChangeHandler changeHandler;
    private EditorValues values;

    private DependencyContainer dependencies;
    private bool exitConfirmed;
    private bool isNewMap;
    private string effectHash;

    public ChartingContainer ChartingContainer { get; set; }

    public Editor(EditorLoader loader, RealmMap realmMap = null, MapInfo map = null)
    {
        this.loader = loader;

        Map = realmMap;
        MapInfo = getEditorMapInfo(map);
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, Storage storage)
    {
        audioClock.Looping = false;
        audioClock.Stop(); // the editor clock will handle this

        isNewMap = MapInfo == null && Map == null;

        if (Map == null)
            Map = mapStore.CreateNew();
        else
        {
            var resources = Map.MapSet.Resources;
            Map = Map.Detach();
            Map.MapSet.Resources = resources;
        }

        MapInfo ??= new EditorMapInfo(new MapMetadata { Mapper = Map.Metadata.Mapper });
        MapInfo.KeyCount = Map.KeyCount;

        backgrounds.AddBackgroundFromMap(Map);
        trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("maps")));

        dependencies.CacheAs(waveform = new Bindable<Waveform>());
        dependencies.CacheAs(changeHandler = new EditorChangeHandler());
        dependencies.CacheAs(values = new EditorValues
        {
            MapInfo = MapInfo,
            MapEvents = MapInfo.MapEvents ?? new EditorMapEvents(),
            Editor = this,
            ActionStack = new EditorActionStack
            {
                // Notifications = notifications
            }
        });

        effectHash = MapUtils.GetHash(values.MapEvents.Save());

        clock = new EditorClock(MapInfo);
        clock.ChangeSource(loadMapTrack());
        dependencies.CacheAs(clock);

        InternalChildren = new Drawable[]
        {
            clock,
            new FluXisContextMenuContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = tabs = new Container
                    {
                        Padding = new MarginPadding { Top = 45, Bottom = 60 },
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            new SetupTab(this),
                            new ChartingTab(this),
                            new TimingTab(this)
                        }
                    }
                }
            },
            menuBar = new EditorMenuBar
            {
                Items = new FluXisMenuItem[]
                {
                    new("File", FontAwesome.Solid.File)
                    {
                        Items = new FluXisMenuItem[]
                        {
                            new("Save", FontAwesome.Solid.Save, () => save()) { Enabled = HasChanges },
                            new FluXisMenuSpacer(),
                            new("Create new difficulty", FontAwesome.Solid.Plus, () => Game.Overlay = new EditorDifficultyCreationPanel
                            {
                                OnCreateNewDifficulty = diffname => createNewDiff(diffname, false),
                                OnCopyDifficulty = diffname => createNewDiff(diffname, true)
                            }),
                            new("Switch to difficulty", FontAwesome.Solid.ExchangeAlt, () => { })
                            {
                                Enabled = () => Map.MapSet.Maps.Count > 1,
                                Items = Map.MapSet.Maps.Where(x => x.ID != Map.ID).Select(x => new FluXisMenuItem(x.Difficulty, () => loader.SwitchTo(x))).ToList()
                            },
                            new("Delete difficulty", FontAwesome.Solid.Trash, () =>
                            {
                                Game.Overlay = new ConfirmDeletionPanel(() =>
                                {
                                    // delete diff
                                    mapStore.DeleteDifficultyFromMapSet(Map.MapSet, Map);

                                    // requery mapset
                                    var set = mapStore.GetFromGuid(Map.MapSet.ID);

                                    // switch to other diff
                                    var other = set.Maps.FirstOrDefault(x => x.ID != Map.ID);
                                    loader.SwitchTo(other);
                                }, itemName: "difficulty");
                            })
                            {
                                Enabled = () => Map.MapSet.Maps.Count > 1
                            },
                            new FluXisMenuSpacer(),
                            new("Export", FontAwesome.Solid.BoxOpen, export),
                            new("Upload", FontAwesome.Solid.Upload, startUpload),
                            new FluXisMenuSpacer(),
                            new("Open Song Folder", FontAwesome.Solid.FolderOpen, () => PathUtils.OpenFolder(MapFiles.GetFullPath(Map.MapSet.GetPathForFile("")))),
                            new FluXisMenuSpacer(),
                            new("Exit", FontAwesome.Solid.Times, MenuItemType.Dangerous, tryExit)
                        }
                    },
                    new("Edit", FontAwesome.Solid.Pen)
                    {
                        Items = new FluXisMenuItem[]
                        {
                            new("Undo", FontAwesome.Solid.Get(0xf0e2), values.ActionStack.Undo) { Enabled = () => values.ActionStack.CanUndo },
                            new("Redo", FontAwesome.Solid.Get(0xf01e), values.ActionStack.Redo) { Enabled = () => values.ActionStack.CanRedo },
                            new FluXisMenuSpacer(),
                            new("Copy", FontAwesome.Solid.Copy, () => ChartingContainer?.Copy()) { Enabled = () => ChartingContainer?.BlueprintContainer.SelectionHandler.SelectedObjects.Any() ?? false },
                            new("Cut", FontAwesome.Solid.Cut, () => ChartingContainer?.Copy(true)) { Enabled = () => ChartingContainer?.BlueprintContainer.SelectionHandler.SelectedObjects.Any() ?? false },
                            new("Paste", FontAwesome.Solid.Paste, () => ChartingContainer?.Paste()),
                            new FluXisMenuSpacer(),
                            new("Delete", FontAwesome.Solid.Trash, () => ChartingContainer?.BlueprintContainer.SelectionHandler.DeleteSelected()),
                            new("Select all", FontAwesome.Solid.ObjectGroup, () => ChartingContainer?.BlueprintContainer.SelectAll())
                        }
                    },
                    new("View", FontAwesome.Solid.Eye)
                    {
                        Items = new FluXisMenuItem[]
                        {
                            new("Waveform opacity", FontAwesome.Solid.Percentage)
                            {
                                Items = new FluXisMenuItem[]
                                {
                                    new("0%", FontAwesome.Solid.Percentage, () => values.WaveformOpacity.Value = 0) { IsActive = () => values.WaveformOpacity.Value == 0 },
                                    new("25%", FontAwesome.Solid.Percentage, () => values.WaveformOpacity.Value = 0.25f) { IsActive = () => values.WaveformOpacity.Value == 0.25f },
                                    new("50%", FontAwesome.Solid.Percentage, () => values.WaveformOpacity.Value = 0.5f) { IsActive = () => values.WaveformOpacity.Value == 0.5f },
                                    new("75%", FontAwesome.Solid.Percentage, () => values.WaveformOpacity.Value = 0.75f) { IsActive = () => values.WaveformOpacity.Value == 0.75f },
                                    new("100%", FontAwesome.Solid.Percentage, () => values.WaveformOpacity.Value = 1) { IsActive = () => values.WaveformOpacity.Value == 1 }
                                }
                            },
                            new("Flash effect underlay", FontAwesome.Solid.LayerGroup, values.FlashUnderlay.Toggle) { IsActive = () => values.FlashUnderlay.Value },
                            new("Flash effect underlay color", FontAwesome.Solid.Palette)
                            {
                                Items = new FluXisMenuItem[]
                                {
                                    new("Dark", () => values.FlashUnderlayColor.Value = FluXisColors.Background1) { IsActive = () => values.FlashUnderlayColor.Value == FluXisColors.Background1 },
                                    new("Light", () => values.FlashUnderlayColor.Value = Colour4.White) { IsActive = () => values.FlashUnderlayColor.Value == Colour4.White }
                                }
                            }
                        }
                    },
                    new("Timing", FontAwesome.Solid.Clock)
                    {
                        Items = new FluXisMenuItem[]
                        {
                            new("Set preview point to current time", FontAwesome.Solid.Stopwatch, () =>
                            {
                                MapInfo.Metadata.PreviewTime = (int)clock.CurrentTime;
                                Map.Metadata.PreviewTime = (int)clock.CurrentTime;
                            })
                        }
                    }
                }
            },
            tabSwitcher = new EditorTabSwitcher
            {
                Children = new EditorTabSwitcherButton[]
                {
                    new()
                    {
                        Text = "Setup",
                        Action = () => changeTab(0)
                    },
                    new()
                    {
                        Text = "Charting",
                        Action = () => changeTab(1)
                    },
                    new()
                    {
                        Text = "Timing",
                        Action = () => changeTab(2)
                    }
                }
            },
            bottomBar = new EditorBottomBar()
        };
    }

    private void createNewDiff(string diffname, bool copy)
    {
        if (diffExists(diffname)) return;

        Game.Overlay.Hide();
        loader.CreateNewDifficulty(Map, MapInfo, diffname, copy);

        bool diffExists(string name)
        {
            if (!Map.MapSet.Maps.Any(x => string.Equals(x.Difficulty, name, StringComparison.CurrentCultureIgnoreCase))) return false;

            notifications.SendError("A difficulty with that name already exists!");
            return true;
        }
    }

    private Track loadMapTrack()
    {
        string path = Map.MapSet?.GetPathForFile(Map.Metadata?.Audio);

        Waveform w = null;

        if (!string.IsNullOrEmpty(path))
        {
            Stream s = trackStore.GetStream(path);
            if (s != null) w = new Waveform(s);
        }

        waveform.Value = w;
        return Map.GetTrack() ?? trackStore.GetVirtual(10000);
    }

    protected override void LoadComplete()
    {
        changeTab(isNewMap ? 0 : 1);

        if (Map.Status >= 100)
        {
            Game.Overlay = new ButtonPanel
            {
                Text = "This map is from another game!",
                SubText = "You can edit and playtest, but not save or upload.",
                Buttons = new ButtonData[]
                {
                    new() { Text = "Okay" }
                }
            };
        }
    }

    public void SortEverything()
    {
        MapInfo.HitObjects.Sort((a, b) => a.Time.CompareTo(b.Time));
        MapInfo.TimingPoints.Sort((a, b) => a.Time.CompareTo(b.Time));
        MapInfo.ScrollVelocities.Sort((a, b) => a.Time.CompareTo(b.Time));
        values.MapEvents.FlashEvents.Sort((a, b) => a.Time.CompareTo(b.Time));
        values.MapEvents.LaneSwitchEvents.Sort((a, b) => a.Time.CompareTo(b.Time));
    }

    private void changeTab(int to)
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
            if (e.Key is >= Key.Number1 and <= Key.Number9)
            {
                int index = e.Key - Key.Number1;

                if (index < tabs.Count)
                    changeTab(index);

                return true;
            }

            switch (e.Key)
            {
                case Key.S:
                    save();
                    return true;
            }
        }

        return false;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (HasChanges() && !exitConfirmed && Map.Status < 100)
        {
            Game.Overlay ??= new ButtonPanel
            {
                Text = "There are unsaved changes.",
                SubText = "Are you sure you want to exit?",
                Width = 800,
                Height = 600,
                ButtonWidth = 280,
                Buttons = new ButtonData[]
                {
                    new()
                    {
                        Text = "Save and exit.",
                        Color = FluXisColors.ButtonGreen,
                        Action = () =>
                        {
                            if (!save()) return;

                            exitConfirmed = true;
                            this.Exit();
                        }
                    },
                    new()
                    {
                        Text = "Exit without saving.",
                        Color = FluXisColors.ButtonRed,
                        Action = () =>
                        {
                            exitConfirmed = true;
                            this.Exit();
                        }
                    },
                    new() { Text = "Nevermind, back to editing." }
                }
            };

            return true;
        }

        exitAnimation();
        clock.Stop();
        audioClock.Seek((float)clock.CurrentTime);
        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e) => enterAnimation();
    public override void OnResuming(ScreenTransitionEvent e) => enterAnimation();
    public override void OnSuspending(ScreenTransitionEvent e) => exitAnimation();

    private void exitAnimation()
    {
        this.FadeOut(200);
        menuBar.MoveToY(-menuBar.Height, 300, Easing.OutQuint);
        tabSwitcher.MoveToY(-menuBar.Height, 300, Easing.OutQuint);
        bottomBar.MoveToY(bottomBar.Height, 300, Easing.OutQuint);
        tabs.ScaleTo(.9f, 300, Easing.OutQuint);
    }

    private void enterAnimation()
    {
        this.FadeInFromZero(200);
        menuBar.MoveToY(0, 300, Easing.OutQuint);
        tabSwitcher.MoveToY(0, 300, Easing.OutQuint);
        bottomBar.MoveToY(0, 300, Easing.OutQuint);
        tabs.ScaleTo(1, 300, Easing.OutQuint);

        // this check wont work 100% of the time, we need a better way of storing the mappers
        if (Map.Metadata.Mapper == fluxel.LoggedInUser?.Username)
            Activity.Value = new UserActivity.Editing();
        else
            Activity.Value = new UserActivity.Modding();
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    private bool save(bool setStatus = true)
    {
        if (Map == null)
            return false;

        if (Map.Status >= 100)
        {
            notifications.SendError("Map is from another game!");
            return false;
        }

        if (MapInfo.HitObjects.Count == 0)
        {
            notifications.SendError("Map has no hit objects!");
            return false;
        }

        if (MapInfo.TimingPoints.Count == 0)
        {
            notifications.SendError("Map has no timing points!");
            return false;
        }

        MapInfo.Sort();

        string effects = values.MapEvents.Save();

        if (string.IsNullOrEmpty(effects))
            MapInfo.EffectFile = "";
        else
        {
            var fileName = string.IsNullOrEmpty(MapInfo.EffectFile) ? $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.ffx" : MapInfo.EffectFile;
            var effectsPath = MapFiles.GetFullPath(Map.MapSet.GetPathForFile(fileName));
            var effectsDir = Path.GetDirectoryName(effectsPath);

            if (!Directory.Exists(effectsDir))
                Directory.CreateDirectory(effectsDir);

            File.WriteAllText(effectsPath, effects);
            MapInfo.EffectFile = fileName;
        }

        // get map as json
        string json = JsonConvert.SerializeObject(MapInfo);
        string hash = MapUtils.GetHash(json);
        string effHash = MapUtils.GetHash(values.MapEvents.Save());

        if (hash == Map.Hash && effHash == effectHash)
        {
            notifications.SendSmallText("Map is already up to date", FontAwesome.Solid.Check);
            return true;
        }

        effectHash = MapUtils.GetHash(values.MapEvents.Save());

        Map.FileName = string.IsNullOrEmpty(Map.FileName) ? $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.fsc" : Map.FileName;

        // write to file
        string path = MapFiles.GetFullPath(Map.MapSet.GetPathForFile(Map.FileName));
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
                if (setStatus) set.SetStatus(-2);
                var prevFile = existingMap.MapSet.GetPathForFile(existingMap.Hash);

                if (prevFile == null)
                {
                    notifications.SendError("Failed to find previous map file!");
                    return;
                }

                existingMap.Hash = hash;
                Map.Hash = hash;
                r.Remove(existingMap.Filters);
                existingMap.Filters = MapUtils.GetMapFilters(MapInfo, values.MapEvents);

                r.Remove(existingMap.Metadata);
                existingMap.Difficulty = MapInfo.Metadata.Difficulty;
                existingMap.MapSet.Cover = Map.MapSet.Cover;
                existingMap.KeyCount = Map.KeyCount;
                existingMap.Metadata = new RealmMapMetadata
                {
                    Title = MapInfo.Metadata.Title,
                    Artist = MapInfo.Metadata.Artist,
                    Mapper = MapInfo.Metadata.Mapper,
                    Source = MapInfo.Metadata.Source,
                    Tags = MapInfo.Metadata.Tags,
                    Background = MapInfo.BackgroundFile,
                    Audio = MapInfo.AudioFile,
                    PreviewTime = MapInfo.Metadata.PreviewTime
                };

                mapStore.UpdateMapSet(mapStore.GetFromGuid(Map.MapSet.ID), set.Detach());
            }
            else
            {
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
                    PreviewTime = MapInfo.Metadata.PreviewTime
                };

                r.Add(Map.MapSet);
                mapStore.AddMapSet(Map.MapSet.Detach());
                Map = Map.Detach(); // unlink from realm again
            }
        });

        notifications.SendSmallText("Saved!", FontAwesome.Solid.Check);
        return true;
    }

    public bool HasChanges()
    {
        string json = JsonConvert.SerializeObject(MapInfo);
        string hash = MapUtils.GetHash(json);
        return hash != Map.Hash;
    }

    private void export()
    {
        if (!save(false)) return;

        mapStore.Export(Map.MapSet, new LoadingNotificationData
        {
            TextLoading = "Exporting mapset...",
            TextSuccess = "Exported!",
            TextFailure = "Failed to export!"
        });
    }

    private void tryExit() => this.Exit(); // TODO: unsaved changes check
    private void sendWipNotification() => notifications.SendText("This is still in development", "Come back later!");

    public void SetKeyMode(int keyMode)
    {
        if (keyMode < Map.KeyCount)
        {
            // check if can be changed
        }

        Map.KeyCount = keyMode;
        MapInfo.KeyCount = keyMode;
        changeHandler.OnKeyModeChanged.Invoke(keyMode);
    }

    public void SetAudio(FileInfo file)
    {
        if (file == null)
            return;

        copyFile(file);
        MapInfo.AudioFile = file.Name;
        Map.Metadata.Audio = file.Name;
        clock.ChangeSource(loadMapTrack());
    }

    public void SetBackground(FileInfo file)
    {
        if (file == null)
            return;

        copyFile(file);
        MapInfo.BackgroundFile = file.Name;
        Map.Metadata.Background = file.Name;
        backgrounds.AddBackgroundFromMap(Map);
    }

    public void SetCover(FileInfo file)
    {
        if (file == null)
            return;

        copyFile(file);
        Map.MapSet.Cover = file.Name;
        MapInfo.CoverFile = file.Name;
    }

    public void SetVideo(FileInfo file)
    {
        if (file == null)
            return;

        copyFile(file);
        MapInfo.VideoFile = file.Name;
    }

    private void copyFile(FileInfo file)
    {
        string path = MapFiles.GetFullPath(Map.MapSet.GetPathForFile(file.Name));
        var dir = Path.GetDirectoryName(path);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.Copy(file.FullName, path, true);
    }

    private void startUpload() => Task.Run(uploadSet);

    private async void uploadSet()
    {
        if (Map.Status >= 100)
        {
            notifications.SendError("Map is from another game!");
            return;
        }

        var overlay = new LoadingPanel
        {
            Text = "Uploading mapset...",
            SubText = "Checking for duplicate diffs..."
        };

        Schedule(() => Game.Overlay = overlay);

        // check for duplicate diffs
        var diffs = Map.MapSet.Maps.Select(m => m.Difficulty).ToList();
        var duplicate = diffs.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();

        if (duplicate.Count > 0)
        {
            notifications.SendError("Cannot upload mapset!", $"Duplicate difficulty names found: {string.Join(", ", duplicate)}");
            return;
        }

        overlay.SubText = "Saving mapset...";

        if (!save(false)) return;

        overlay.SubText = "Uploading mapset...";

        var realmMapSet = mapStore.GetFromGuid(Map.MapSet.ID);
        var path = mapStore.Export(realmMapSet.Detach(), new LoadingNotificationData(), false);
        var buffer = await File.ReadAllBytesAsync(path);

        var request = fluxel.CreateAPIRequest(Map.MapSet.OnlineID != -1 ? $"/map/{Map.MapSet.OnlineID}/update" : "/maps/upload", HttpMethod.Post);
        request.AddFile("file", buffer);
        request.UploadProgress += (l1, l2) => overlay.SubText = $"Uploading mapset... {(int)((float)l1 / l2 * 100)}%";
        await request.PerformAsync();

        overlay.SubText = "Reading server response...";

        var json = request.GetResponseString();
        var response = JsonConvert.DeserializeObject<APIResponse<APIMapSet>>(json);

        if (response.Status != 200)
        {
            notifications.SendError(response.Message);
            Schedule(overlay.Hide);
            return;
        }

        overlay.SubText = "Updating mapset...";

        realm.RunWrite(r =>
        {
            var set = r.Find<RealmMapSet>(Map.MapSet.ID);
            set.OnlineID = Map.MapSet.OnlineID = response.Data.Id;
            set.SetStatus(response.Data.Status);
            Map.MapSet.SetStatus(response.Data.Status);

            for (var index = 0; index < set.Maps.Count; index++)
            {
                var onlineMap = response.Data.Maps[index];
                var map = set.Maps.First(m => m.Difficulty == onlineMap.Difficulty);
                var loadedMap = Map.MapSet.Maps.First(m => m.Difficulty == onlineMap.Difficulty);

                map.OnlineID = loadedMap.OnlineID = onlineMap.Id;
            }
        });

        Schedule(overlay.Hide);
    }

    private EditorMapInfo getEditorMapInfo(MapInfo map)
    {
        if (map == null) return null;

        var eMap = EditorMapInfo.FromMapInfo(map.Clone());
        eMap.Map = Map;
        return eMap;
    }
}
