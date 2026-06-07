using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Configuration.Experiments;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Background;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Context;
using fluXis.Graphics.UserInterface.Files;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Map.Structures.Events;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Requests.Maps;
using fluXis.Online.API.Requests.MapSets;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Overlay.Wiki;
using fluXis.Plugins;
using fluXis.Plugins.Capabilities;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Input;
using fluXis.Screens.Edit.Modding;
using fluXis.Screens.Edit.Tabs;
using fluXis.Screens.Edit.Tabs.Charting;
using fluXis.Screens.Edit.Tabs.Storyboarding;
using fluXis.Screens.Edit.Tabs.Verify.Checks;
using fluXis.Screens.Edit.UI;
using fluXis.Screens.Edit.UI.BottomBar;
using fluXis.Screens.Edit.UI.MenuBar;
using fluXis.Screens.Edit.UI.Panels;
using fluXis.Screens.Edit.UI.TabSwitcher;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Scripting;
using fluXis.Skinning.Default;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using ManagedBass.Fx;
using Midori.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK.Input;

namespace fluXis.Screens.Edit;

public partial class Editor : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>, IKeyBindingHandler<EditorKeybinding>
{
    public override bool ShowToolbar => false;
    public override float BackgroundDim => BindableBackgroundDim.Value;
    public override float BackgroundBlur => BindableBackgroundBlur.Value;
    public override bool AllowMusicControl => false;
    public override bool ApplyValuesAfterLoad => true;
    public override float ParallaxStrength => 0f;
    public override bool ShowCursor => false;

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private WikiOverlay wiki { get; set; }

    [Resolved(CanBeNull = true)]
    private GlobalFFTProcessor fftProcessor { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PluginManager plugins { get; set; }

    /// <summary>
    /// overwrites the tab the editor opens with
    /// </summary>
    public int StartTabIndex { get; init; } = -1;

    private ITrackStore trackStore { get; set; }

    private EditorLoader loader { get; }

    private Container<EditorTab> tabs;
    private int currentTab;

    public Bindable<Waveform> Waveform { get; private set; }
    public EditorMap EditorMap { get; private set; }

    public EditorClock EditorClock { get; private set; }
    private EditorSettings settings;
    private EditorActionStack actionStack;

    private long openTime;
    private long lastSaveTime;

    private AudioFilter lowPass;
    private AudioFilter highPass;
    private bool lowPassEnabled;
    private bool highPassEnabled;

    private DependencyContainer dependencies;
    private bool exitConfirmed;
    private bool isNewMap;
    private bool isUploading;

    private string lastMapHash;
    private string lastEffectHash;
    private string lastStoryboardHash;

    private bool canSave => EditorMap.RealmMap.StatusInt < 100;

    public bool HasUnsavedChanges
    {
        get
        {
            if (!canSave)
                return false;

            return EditorMap.MapEventsHash != lastEffectHash
                   || EditorMap.MapInfoHash != lastMapHash
                   || EditorMap.StoryboardHash != lastStoryboardHash;
        }
    }

    public Bindable<float> BindableBackgroundDim { get; private set; }
    public Bindable<float> BindableBackgroundBlur { get; private set; }

    public string MapSetPath { get; private set; }

    public ChartingContainer ChartingContainer { get; set; }
    private VerifyTab verifyTab;

    private EditorKeybindingContainer keybinds;
    private EditorKeymapOverlay keymapOverlay;
    private EditorModding modding;
    private EditorOsd osd;

    private Bindable<bool> autosave;

    public Editor(EditorLoader loader, RealmMap realmMap = null, EditorMap.EditorMapInfo map = null)
    {
        this.loader = loader;
        EditorMap = new EditorMap(map, realmMap, LoadComponent, Scheduler);
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, GameHost host, FluXisConfig config, ExperimentConfigManager experiments)
    {
        BindableBackgroundDim = config.GetBindable<float>(FluXisSetting.EditorDim);
        BindableBackgroundBlur = config.GetBindable<float>(FluXisSetting.EditorBlur);

        autosave = config.GetBindable<bool>(FluXisSetting.EditorAutoSave);

        globalClock.Looping = false;

        EditorMap.Panels = panels;
        isNewMap = EditorMap.IsNew;

        if (EditorMap.RealmMap == null)
        {
            EditorMap.RealmMap = mapStore.CreateNew();
        }
        else
        {
            var resources = EditorMap.RealmMap.MapSet.Resources;
            EditorMap.RealmMap = EditorMap.RealmMap.Detach();
            EditorMap.RealmMap.MapSet.Resources = resources;
        }

        EditorMap.MapInfo ??= new EditorMap.EditorMapInfo(new MapMetadata { Mapper = EditorMap.RealmMap.Metadata.Mapper }) { NewLaneSwitchLayout = true, RealmEntry = EditorMap.RealmMap };
        EditorMap.MapInfo.MapEvents ??= new MapEvents();
        EditorMap.MapInfo.Storyboard ??= new Storyboard { Version = Storyboard.LATEST_VERSION };

        EditorMap.SetupWatcher();
        EditorMap.SetupNotifiers();

        backgrounds.AddBackgroundFromMap(EditorMap.RealmMap);
        trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(host.Storage.GetStorageForDirectory("maps")));

        dependencies.CacheAs(this);

        keybinds = new EditorKeybindingContainer(this, config.GetBindable<string>(FluXisSetting.EditorKeymap), host);
        dependencies.CacheAs(keybinds);

        dependencies.CacheAs(EditorMap);
        dependencies.CacheAs<ICustomColorProvider>(EditorMap.MapInfo.Colors);
        dependencies.CacheAs(Waveform = new Bindable<Waveform>());
        dependencies.CacheAs(actionStack = new EditorActionStack(EditorMap) { NotificationManager = notifications });
        dependencies.CacheAs(modding = new EditorModding());
        dependencies.CacheAs(settings = new EditorSettings(keybinds)
        {
            ShowSamples = config.GetBindable<bool>(FluXisSetting.EditorShowSamples)
        });

        updateStateHash();

        EditorClock = new EditorClock(EditorMap.MapInfo) { SnapDivisor = settings.SnapDivisorBindable };
        EditorClock.ChangeSource(loadMapTrack());
        dependencies.CacheAs(EditorClock);
        dependencies.CacheAs<IBeatSyncProvider>(EditorClock);

        dependencies.CacheAs(new EditorSnapProvider(EditorMap, settings, EditorClock));

        MapSetPath = MapFiles.GetFullPath($"{EditorMap.MapSet.ID}");

        if (!Directory.Exists(MapSetPath))
            Directory.CreateDirectory(MapSetPath);

        var scripts = new ScriptStorage(MapSetPath);
        EditorMap.ScriptChanged += _ =>
        {
            osd.DisplayText("Reloaded Scripts!");
            scripts.Reload();
        };
        dependencies.CacheAs(scripts);

        var tabList = new List<EditorTab>
        {
            new SetupTab(),
            new ChartingTab(),
            new DesignTab(),
            new StoryboardTab(),
            new VerifyTab()
            // new WipEditorTab(FontAwesome6.Solid.Music, "Hitsounding", "Soon you'll be able to edit volume of hitsounds and other stuff here.")
        };

        var capabilities = plugins?.Plugins?.GetCapabilities<IEditorCapability>().ToList() ?? [];

        capabilities.ForEach(c =>
        {
            var pluginTabs = c.CustomTabs;
            if (tabs?.Count == 0 || pluginTabs == null) return;

            tabList.AddRange(pluginTabs);
        });

        verifyTab = tabList.OfType<VerifyTab>().First();

        InternalChild = keybinds.WithChildren(new Drawable[]
        {
            lowPass = new AudioFilter(audioManager.TrackMixer),
            highPass = new AudioFilter(audioManager.TrackMixer, BQFType.HighPass),
            EditorClock,
            modding,
            dependencies.CacheAsAndReturn(new Hitsounding(EditorMap.RealmMap.MapSet, EditorMap.MapInfo.HitSoundFades, EditorClock.RateBindable)
            {
                DirectVolume = true,
                Clock = EditorClock
            }),
            new FluXisContextMenuContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = 45, Bottom = 60 },
                            Child = tabs = new Container<EditorTab>
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Children = tabList
                            }
                        },
                        new EditorModdingOverlay(),
                        new EditorMenuBar
                        {
                            Items = new FluXisMenuItem[]
                            {
                                new MenuExpandItem("File", Phosphor.Bold.File, new FluXisMenuItem[]
                                {
                                    new MenuActionItem("Save", Phosphor.Bold.FloppyDisk, () => save()) { IsEnabled = () => HasUnsavedChanges },
                                    new MenuToggleItem("Auto Save", Phosphor.Bold.FloppyDisk, autosave),
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Create new difficulty...", Phosphor.Bold.Plus, () => panels.Content = new EditorDifficultyCreationPanel
                                    {
                                        OnCreate = createNewDiff
                                    }) { IsEnabled = () => canSave },
                                    new MenuActionItem("Switch to difficulty", Phosphor.Bold.ArrowsLeftRight, () => { })
                                    {
                                        IsEnabled = () => EditorMap.MapSet.Maps.Count > 1,
                                        Items = EditorMap.MapSet.Maps.Where(x => x.ID != EditorMap.RealmMap.ID)
                                                         .Select(x => new MenuActionItem(x.Difficulty, Phosphor.Bold.Circle, () => loader.SwitchTo(x))).ToList()
                                    },
                                    new MenuActionItem("Delete difficulty...", Phosphor.Bold.Trash, () =>
                                    {
                                        panels.Content = new ConfirmDeletionPanel(() =>
                                        {
                                            // delete diff
                                            mapStore.DeleteDifficulty(EditorMap.MapSet, EditorMap.RealmMap);

                                            // requery mapset
                                            var set = mapStore.GetFromGuid(EditorMap.MapSet.ID);

                                            // switch to other diff
                                            var other = set.Maps.FirstOrDefault(x => x.ID != EditorMap.RealmMap.ID);
                                            loader.SwitchTo(other);
                                        }, itemName: "difficulty");
                                    })
                                    {
                                        IsEnabled = () => EditorMap.MapSet.Maps.Count > 1 && canSave
                                    },
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Export", Phosphor.Bold.Package, export),
                                    new MenuActionItem("Upload...", Phosphor.Bold.Upload, startUpload) { IsEnabled = () => canSave && api.IsLoggedIn },
                                    new MenuActionItem("Submit to Queue...", Phosphor.Bold.Upload, submitToQueue) { IsEnabled = () => EditorMap.MapSet.OnlineID > 0 && api.IsLoggedIn },
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Open Song Folder", Phosphor.Bold.FolderOpen, openFolder),
                                    experiments.Get<bool>(ExperimentConfig.LrcFeatures) ? new MenuActionItem("Export notes as .lrc", Phosphor.Bold.List, exportNotes) : null,
                                    experiments.Get<bool>(ExperimentConfig.LrcFeatures) ? new MenuActionItem("Import .lrc as notes", Phosphor.Bold.List, importNotes) : null,
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Exit", Phosphor.Bold.DoorOpen, MenuItemType.Dangerous, tryExit)
                                }.Where(x => x != null)),
                                new MenuExpandItem("Edit", Phosphor.Bold.PencilSimple, new FluXisMenuItem[]
                                {
                                    new MenuActionItem("Undo", Phosphor.Bold.ArrowCounterClockwise, actionStack.Undo) { IsEnabled = () => actionStack.CanUndo },
                                    new MenuActionItem("Redo", Phosphor.Bold.ArrowClockwise, actionStack.Redo) { IsEnabled = () => actionStack.CanRedo },
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Copy", Phosphor.Bold.Copy, () => ChartingContainer?.Copy())
                                        { IsEnabled = () => ChartingContainer?.BlueprintContainer.SelectionHandler.SelectedObjects.Any() ?? false },
                                    new MenuActionItem("Cut", Phosphor.Bold.Scissors, () => ChartingContainer?.Copy(true))
                                        { IsEnabled = () => ChartingContainer?.BlueprintContainer.SelectionHandler.SelectedObjects.Any() ?? false },
                                    new MenuActionItem("Paste", Phosphor.Bold.Clipboard, () => ChartingContainer?.Paste()),
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Apply Offset...", Phosphor.Bold.Clock, applyOffset),
                                    new MenuActionItem("Flip Selection", Phosphor.Bold.ArrowsHorizontal, () => ChartingContainer?.FlipSelection())
                                        { IsEnabled = () => ChartingContainer?.CanFlipSelection ?? false },
                                    new MenuActionItem("Shuffle Selection", Phosphor.Bold.ShuffleAngular, () => ChartingContainer?.ShuffleSelection())
                                        { IsEnabled = () => ChartingContainer?.CanShuffleSelection ?? false },
                                    new MenuActionItem("Re-snap all notes", Phosphor.Bold.ArrowsClockwise, () => ChartingContainer?.ReSnapAll()),
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Select all", Phosphor.Bold.SelectionAll, () => ChartingContainer?.BlueprintContainer.SelectAll()),
                                    new MenuActionItem("Delete", Phosphor.Bold.Trash, () => ChartingContainer?.BlueprintContainer.SelectionHandler.DeleteSelected()),
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Editor Keymap...", Phosphor.Bold.Keyboard, () => keymapOverlay.Show())
                                }),
                                new MenuExpandItem("View", Phosphor.Bold.Eye, createView()),
                                new MenuExpandItem("Timing", Phosphor.Bold.Clock, new FluXisMenuItem[]
                                {
                                    new MenuActionItem("Set preview point to current time", Phosphor.Bold.Timer, () =>
                                    {
                                        EditorMap.MapInfo.Metadata.PreviewTime
                                            = EditorMap.RealmMap.Metadata.PreviewTime
                                                = (int)EditorClock.CurrentTime;
                                    })
                                }),
                                new MenuExpandItem("Audio", Phosphor.Bold.SpeakerHigh, new FluXisMenuItem[]
                                {
                                    new MenuToggleItem("Enable Low Pass filter", Phosphor.Bold.CaretDown, () =>
                                    {
                                        lowPassEnabled = !lowPassEnabled;
                                        lowPass.CutoffTo(lowPassEnabled ? AudioFilter.MIN : AudioFilter.MAX, 400);
                                    }, () => lowPassEnabled),
                                    new MenuToggleItem("Enable High Pass filter", Phosphor.Bold.CaretUp, () =>
                                    {
                                        highPassEnabled = !highPassEnabled;
                                        highPass.CutoffTo(highPassEnabled ? 300 : 0, 400);
                                    }, () => highPassEnabled)
                                }),
                                new MenuActionItem("Wiki", Phosphor.Bold.Book, openHelp)
                            }
                        },
                        new EditorTabSwitcher
                        {
                            ChildrenEnumerable = tabs.Select(x => new EditorTabSwitcherButton(x.Icon, x.TabName, () => changeTab(tabs.IndexOf(x))))
                        },
                        new EditorBottomBar()
                    }
                }
            },
            keymapOverlay = new EditorKeymapOverlay(keybinds),
            dependencies.CacheAsAndReturn(osd = new EditorOsd())
        });

        return;

        FluXisMenuItem[] createView()
        {
            var list = new List<FluXisMenuItem>
            {
                new MenuExpandItem("Background Dim", Phosphor.Bold.Image, createPercentItems(() => BindableBackgroundDim.Value, v => BindableBackgroundDim.Value = v)),
                new MenuExpandItem("Background Blur", Phosphor.Bold.Aperture, createPercentItems(() => BindableBackgroundBlur.Value, v => BindableBackgroundBlur.Value = v)),
                new MenuSpacerItem(),
                new MenuExpandItem("Waveform opacity", Phosphor.Bold.Waveform, createPercentItems(() => settings.WaveformOpacity.Value, v => settings.WaveformOpacity.Value = v)),
                new MenuSpacerItem(),
                new MenuToggleItem("Show sample on notes", Phosphor.Bold.Stack, settings.ShowSamples),
                new MenuSpacerItem(),
                new MenuToggleItem("Force 16:9 Ratio", Phosphor.Bold.Rectangle, settings.ForceAspectRatio),
                new MenuToggleItem("Compact Sidebar", Phosphor.Bold.ArrowsInLineVertical, config.GetBindable<bool>(FluXisSetting.EditorCompactMode)),
            };

            if (experiments.Get<bool>(ExperimentConfig.ModView))
                list.Add(new MenuToggleItem("Toggle ModView", Phosphor.Bold.PencilSimple, () => modding.Toggle(), () => modding.IsActive));

            return list.ToArray();
        }
    }

    private FluXisMenuItem[] createPercentItems(Func<float> get, Action<float> set)
    {
        float[] values = { 0, .2f, .4f, .6f, .8f, 1 };
        return values.Select(x => new MenuToggleItem($"{x * 100:0}%", Phosphor.Bold.Percent, () => set(x), () => Math.Abs(get() - x) < .01f)).ToArray<FluXisMenuItem>();
    }

    private void updateStateHash()
    {
        lastMapHash = EditorMap.MapInfoHash;
        lastEffectHash = EditorMap.MapEventsHash;
        lastStoryboardHash = EditorMap.StoryboardHash;
    }

    private void applyOffset() => panels.Add(new FormPanel<EditorApplyOffset>(Phosphor.Bold.Clock, "Apply offset to map", new EditorApplyOffset(), (panel, offset) =>
    {
        if (!offset.Offset.TryParseDoubleInvariant(out var result))
            return false;

        actionStack.Add(new ApplyOffsetAction(result));
        return true;
    }));

    private void createNewDiff(CreateNewMapParameters param)
    {
        if (diffExists(param.DifficultyName))
            return;

        panels.Content.Hide();
        loader.CreateNewDifficulty(EditorMap.RealmMap, EditorMap.MapInfo, param);

        bool diffExists(string name)
        {
            if (!EditorMap.MapSet.Maps.Any(x => string.Equals(x.Difficulty, name, StringComparison.CurrentCultureIgnoreCase)))
                return false;

            notifications.SendError("A difficulty with that name already exists!");
            return true;
        }
    }

    private Track loadMapTrack()
    {
        string path = EditorMap.MapSet?.GetPathForFile(EditorMap.RealmMap.Metadata?.Audio);

        Waveform w = null;

        if (!string.IsNullOrEmpty(path))
        {
            Stream s = trackStore.GetStream(path);
            if (s != null) w = new Waveform(s);
        }

        Waveform.Value = w;
        return EditorMap.RealmMap.GetTrack() ?? trackStore.GetVirtual(10000);
    }

    protected override void LoadComplete()
    {
        openTime = lastSaveTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var idx = isNewMap ? 0 : 1;

        if (StartTabIndex != -1)
            idx = StartTabIndex;

        changeTab(idx);

        if (!canSave)
        {
            panels.Content = new SingleButtonPanel(
                Phosphor.Bold.Warning,
                "This map is from another game!",
                "You can edit and playtest, but not save or upload.");
        }

        BindableBackgroundDim.BindValueChanged(updateDim, true);
        BindableBackgroundBlur.BindValueChanged(updateBlur, true);

        EditorMap.AudioChanged += () => EditorClock.ChangeSource(loadMapTrack());
        EditorMap.BackgroundChanged += () => backgrounds.AddBackgroundFromMap(EditorMap.RealmMap);

        EditorMap.ScriptWatcher.Enable();

        var capabilities = plugins?.Plugins?.GetCapabilities<IEditorCapability>().ToList() ?? [];
        capabilities.ForEach(c => c?.OnEditorLoaded(editorMap));
    }

    protected override void Update()
    {
        base.Update();

        if (autosave.Value)
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (now - lastSaveTime > 1000 * 60 * 5) save();
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        EditorClock.Stop();
        BindableBackgroundDim.UnbindAll();
        BindableBackgroundBlur.UnbindAll();
        EditorMap.ScriptWatcher.Dispose();
    }

    private void updateDim(ValueChangedEvent<float> e) => backgrounds.SetDim(e.NewValue);
    private void updateBlur(ValueChangedEvent<float> e) => backgrounds.SetBlur(e.NewValue);

    public void ChangeToTab<T>([CanBeNull] Action<T> act = null) where T : EditorTab =>
        ChangeToTab(typeof(T), x => act?.Invoke(x as T));

    public void ChangeToTab(Type tab, [CanBeNull] Action<EditorTab> act = null)
    {
        var target = tabs.FirstOrDefault(x => x.GetType() == tab) ?? throw new InvalidOperationException("Tab not in editor.");
        changeTab(tabs.IndexOf(target));

        if (target.HasLoading)
            target.ScheduleAfterLoad(() => act?.Invoke(target));
        else
            act?.Invoke(target);
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
            var tab = tabs.Children[i];

            if (i == currentTab)
                tab.Show();
            else
                tab.Hide();
        }
    }

    private void openHelp()
    {
        if (wiki != null)
        {
            wiki.NavigateTo("/editor");
            return;
        }

        Game.OpenLink($"{api.Endpoint.WikiRootUrl}/editor");
    }

    private void openFolder() => MapFiles.PresentExternally(EditorMap.RealmMap);

    private void exportNotes()
    {
        var sb = new StringBuilder();
        var extended = false;

        foreach (var ev in EditorMap.MapEvents.NoteEvents)
        {
            var time = TimeUtils.Format(ev.Time);
            var text = ev.Content ?? string.Empty;

            if (text.StartsWith('+'))
            {
                text = text[1..];
                extended = true;
                sb.Append($"<{time}>{text}");
            }
            else
            {
                sb.AppendLine();
                sb.Append($"[{time}] {text}");
            }
        }

        var path = MapFiles.GetFullPath(EditorMap.MapSet.GetPathForFile($"lyrics.{(extended ? "elrc" : "lrc")}"));
        File.WriteAllText(path, sb.ToString().Trim());
    }

    private void importNotes()
    {
        var timeTagRegex = new Regex(@"\[(\d{2}):(\d{2})\.(\d{2,3})\]", RegexOptions.Compiled);

        panels.Content = new FileSelect
        {
            AllowedExtensions = [".lrc"],
            OnFileSelected = f =>
            {
                var lines = File.ReadAllLines(f.FullName);

                EditorMap.MapEvents.NoteEvents.ToList().ForEach(x => EditorMap.Remove(x));

                foreach (var line in lines)
                {
                    var matches = timeTagRegex.Matches(line);

                    if (matches.Count == 0)
                        continue;

                    var text = timeTagRegex.Replace(line, "").Trim();

                    foreach (Match match in matches)
                    {
                        var minutes = int.Parse(match.Groups[1].Value);
                        var seconds = int.Parse(match.Groups[2].Value);

                        var hundredGroup = match.Groups[3].Value;
                        var hundredths = int.Parse(hundredGroup);
                        hundredths *= hundredGroup.Length == 3 ? 1 : 10;

                        var ms = hundredths + seconds * 1000 + minutes * 1000 * 60;
                        EditorMap.Add(new NoteEvent { Time = ms, Content = text });
                    }
                }
            }
        };
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
        }

        return false;
    }

    public bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e)
    {
        switch (e.Action)
        {
            case EditorKeybinding.Save:
                save();
                return true;

            case EditorKeybinding.OpenFolder:
                openFolder();
                return true;

            case EditorKeybinding.OpenHelp:
                openHelp();
                return true;

            case EditorKeybinding.PreviousNote:
                seekToNote(-1);
                return true;

            case EditorKeybinding.NextNote:
                seekToNote(1);
                return true;
        }

        return false;

        void seekToNote(int direction)
        {
            var accurate = EditorClock.CurrentTimeAccurate;

            var note = direction > 0
                ? EditorMap.MapEvents.NoteEvents.FirstOrDefault(n => n.Time > accurate)
                : EditorMap.MapEvents.NoteEvents.LastOrDefault(n => n.Time < accurate);

            if (note is not null)
                EditorClock.SeekSmoothly(note.Time);
        }
    }

    public void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) { }

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

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (HasUnsavedChanges && !exitConfirmed)
        {
            panels.Content ??= new UnsavedChangesPanel(() =>
            {
                if (!save())
                    return;

                exitConfirmed = true;
                this.Exit();
            }, () =>
            {
                exitConfirmed = true;
                this.Exit();
            });

            return true;
        }

        if (isNewMap) // delete the map if it was new and not saved
            mapStore.DeleteMapSet(EditorMap.MapSet);

        if (fftProcessor is not null) fftProcessor.Enabled.Value = true;

        // I hate this but it works. I hate this but it works. I hate this but it works.
        this.Delay(EditorLoader.DURATION * .98f).FadeOut();

        lowPass.CutoffTo(AudioFilter.MAX, 400);
        highPass.CutoffTo(0, 400);
        EditorClock.Track.Value.VolumeTo(0, EditorLoader.DURATION);
        globalClock.Seek((float)EditorClock.CurrentTime);
        panels.Content?.Hide();
        setSystemCursorVisibility(false);
        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        // TODO: probably check back on this later if we add visualizations to editor in the future
        if (fftProcessor is not null) fftProcessor.Enabled.Value = false;
        enterAnimation();
        FinishTransforms(true);
    }

    public override void OnResuming(ScreenTransitionEvent e) => enterAnimation();
    public override void OnSuspending(ScreenTransitionEvent e) => exitAnimation();

    private void setSystemCursorVisibility(bool visible)
    {
        if (Game is null) return;

        Game.Window.CursorState = visible ? CursorState.Default : CursorState.Hidden;
    }

    private void exitAnimation()
    {
        setSystemCursorVisibility(false);
        lowPass.CutoffTo(AudioFilter.MAX, Styling.TRANSITION_MOVE);
        highPass.CutoffTo(0, Styling.TRANSITION_MOVE);
        this.ScaleTo(1.2f, Styling.TRANSITION_MOVE, Easing.OutQuint).FadeOut(Styling.TRANSITION_FADE);
    }

    private void enterAnimation()
    {
        setSystemCursorVisibility(true);

        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
        {
            this.ScaleTo(1f).FadeInFromZero(Styling.TRANSITION_FADE);
            lowPass.CutoffTo(lowPassEnabled ? AudioFilter.MIN : AudioFilter.MAX, Styling.TRANSITION_MOVE);
            highPass.CutoffTo(highPassEnabled ? 300 : 0, Styling.TRANSITION_MOVE);
        }

        // this check won't work 100% of the time, we need a better way of storing the mappers
        if (EditorMap.RealmMap.Metadata.Mapper == api.User.Value?.Username)
            Activity.Value = new UserActivity.Editing(openTime);
        else
            Activity.Value = new UserActivity.Modding(openTime);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    private bool save(bool setStatus = true)
    {
        if (!canSave)
        {
            notifications.SendError("Map is from another game!");
            return false;
        }

        if (EditorMap.MapInfo.TimingPoints.Count == 0)
        {
            notifications.SendError("Map has no timing points!");
            return false;
        }

        EditorMap.Sort();

        if (!HasUnsavedChanges)
        {
            notifications.SendSmallText("Map is already up to date", Phosphor.Bold.Check);
            return true;
        }

        EditorMap.ScriptWatcher.Disable();

        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        EditorMap.MapInfo.TimeInEditor += now - lastSaveTime;

        mapStore.Save(EditorMap.RealmMap, EditorMap.MapInfo, EditorMap.MapEvents, EditorMap.Storyboard, setStatus);
        Scheduler.ScheduleOnceIfNeeded(() => mapStore.UpdateMapSet(mapStore.GetFromGuid(EditorMap.MapSet.ID), EditorMap.MapSet));

        isNewMap = false;
        updateStateHash();
        notifications.SendSmallText("Saved!", Phosphor.Bold.Check);
        lastSaveTime = now;

        EditorMap.ScriptWatcher.Enable();
        return true;
    }

    private void export()
    {
        if (!save(false)) return;

        mapStore.Export(EditorMap.MapSet, new TaskNotificationData
        {
            Text = $"{EditorMap.MapInfo.Metadata.Title} - {EditorMap.MapInfo.Metadata.Artist}",
            TextWorking = "Exporting..."
        });
    }

    private void tryExit() => this.Exit(); // TODO: unsaved changes check

    private void submitToQueue()
    {
        if (EditorMap.MapSet.OnlineID <= 0)
            return;

        var panel = new EditorUploadOverlay
        {
            Text = "Submitting to queue...",
            SubText = "Please wait..."
        };

        panels.Content = panel;

        var req = new MapSetSubmitQueueRequest(EditorMap.MapSet.OnlineID);
        req.Success += _ =>
        {
            notifications.SendSmallText("Submitted to queue!");
            panel.Hide();
        };
        req.Failure += ex => panels.Replace(new SingleButtonPanel(
            Phosphor.Bold.Warning,
            "Failed to submit!",
            ex.Message
        ));

        api.PerformRequestAsync(req);
    }

    private void startUpload()
    {
        if (isUploading)
            return;

        if (!api.IsLoggedIn)
        {
            notifications.SendError("You need to be logged in to upload maps!");
            return;
        }

        if (!canSave)
        {
            notifications.SendError("Map is from another game!");
            return;
        }

        var isUpdate = EditorMap.MapSet.OnlineID > 0;

        if (isUpdate)
        {
            panels.Content = new ButtonPanel
            {
                Icon = Phosphor.Bold.Warning,
                Text = "You are about to update a mapset!",
                SubText = "Are you sure you want to continue?\nThis will wipe scores of updated maps.",
                Buttons = new ButtonData[]
                {
                    new DangerButtonData(LocalizationStrings.General.PanelGenericConfirm, () => this.Delay(200).Schedule(() => run())),
                    new CancelButtonData()
                }
            };
        }
        else
        {
            Schedule(() =>
            {
                // temporary container to fade in
                var container = new Container();
                container.OnLoadComplete += c => c.FadeInFromZero(400);
                panels.Content = container;
            });

            // check for already uploaded maps
            // and ask if the user wants to replace that one instead
            var req = new MapLookupRequest
            {
                MapperID = api.User.Value.ID,
                Title = EditorMap.MapInfo.Metadata.Title,
                Artist = EditorMap.MapInfo.Metadata.Artist
            };
            req.Failure += _ => run(); // just run the upload if the request fails

            req.Success += res =>
            {
                // nothing found
                if (!res.Success)
                {
                    run();
                    return;
                }

                panels.Content = new ButtonPanel
                {
                    Icon = Phosphor.Bold.Warning,
                    Text = "You already have a mapset with the same title and artist uploaded!",
                    CreateSubText = flow =>
                    {
                        flow.AddText("Do you want to update that one mapset instead?");
                        flow.NewParagraph();
                        flow.NewParagraph();
                        flow.AddText<ClickableFluXisSpriteText>("Click here to view the mapset.", t =>
                        {
                            t.Colour = Theme.Highlight;
                            t.Action = () => Game.OpenLink($"{api.Endpoint.WebsiteRootUrl}/set/{res.Data.SetID}", true);
                        });
                    },
                    Buttons = new ButtonData[]
                    {
                        new PrimaryButtonData("Yes, update that mapset.", () =>
                        {
                            isUpdate = true;
                            this.Delay(200).Schedule(() => run(res.Data.ID));
                        }),
                        new SecondaryButtonData("No, upload as new.", () => this.Delay(200).Schedule(() => run())),
                        new CancelButtonData("Wait, go back to editing.")
                    }
                };
            };

            api.PerformRequest(req);
        }

        void run(long id = -1) => Task.Run(() =>
        {
            if (isUploading)
                return;

            isUploading = true;
            uploadSet(isUpdate, id);
            isUploading = false;
        });
    }

    private async void uploadSet(bool isUpdate, long setID = -1)
    {
        try
        {
            var overlay = new EditorUploadOverlay
            {
                Text = isUpdate ? "Updating mapset..." : "Uploading mapset...",
                SubText = "Checking for issues..."
            };

            Schedule(() => panels.Content = overlay);

            var files = new Dictionary<string, int>();

            foreach (var map in EditorMap.MapSet.Maps)
            {
                overlay.SubText = $"Checking for issues in '{map.Difficulty}'...";

                var results = verifyTab.RunVerify(new BasicVerifyContext(map, LoadComponent));
                files[map.Difficulty] = results.ProblematicIssues;
            }

            var problems = files.Sum(x => x.Value);

            if (problems > 0)
            {
                var builder = new StringBuilder();
                builder.AppendLine($"You have {problems} problematic issue(s) in your mapset.");
                builder.AppendLine("Please fix them before uploading.");
                builder.AppendLine("You can check the issues in the verify tab.");
                builder.AppendLine();

                foreach (var kvp in files)
                    builder.AppendLine($"{kvp.Key}: {kvp.Value} issue(s)");

                Schedule(() => panels.Replace(new SingleButtonPanel(
                    Phosphor.Bold.Warning,
                    "Issues found!",
                    builder.ToString()
                )));

                return;
            }

            overlay.SubText = "Saving...";

            if (!save(false))
            {
                Schedule(() => panels.Content?.Hide());
                return;
            }

            overlay.SubText = "Exporting...";

            var realmMapSet = mapStore.GetFromGuid(EditorMap.MapSet.ID);
            var path = mapStore.Export(realmMapSet.Detach(), new TaskNotificationData(), false);
            var buffer = await File.ReadAllBytesAsync(path);

            overlay.SubText = "0%";

            if (setID == -1 && isUpdate)
                setID = EditorMap.MapSet.OnlineID;

            var request = new MapSetUploadRequest(buffer, setID);
            request.Progress += (l1, l2) => overlay.SubText = $"{l1.FormatBytes()}/{l1.FormatBytes()} {Math.Round((float)l1 / l2 * 100, 2).ToStringInvariant("00.00")}%";
            await api.PerformRequestAsync(request);

            overlay.SubText = "Reading response...";

            if (!request.IsSuccessful)
            {
                Schedule(() => panels.Replace(new SingleButtonPanel(
                    Phosphor.Bold.Warning,
                    $"Failed up {(isUpdate ? "update" : "upload")} mapset!",
                    request.FailReason?.Message ?? APIRequest.UNKNOWN_ERROR
                )));
                return;
            }

            overlay.SubText = "Assigning IDs...";

            realm.RunWrite(r =>
            {
                var set = r.Find<RealmMapSet>(EditorMap.MapSet.ID);
                set.OnlineID = EditorMap.MapSet.OnlineID = request.Response!.Data.ID;
                set.SetStatus(request.Response.Data.Status);
                EditorMap.MapSet.SetStatus(request.Response.Data.Status);

                foreach (var onlineMap in request.Response.Data.Maps)
                {
                    var map = set.Maps.First(m => m.FileName == onlineMap.FileName);
                    var loadedMap = EditorMap.MapSet.Maps.First(m => m.FileName == onlineMap.FileName);

                    map.OnlineID = loadedMap.OnlineID = onlineMap.ID;
                }

                var detatch = set.Detach();
                Schedule(() => mapStore.UpdateMapSet(mapStore.GetFromGuid(EditorMap.MapSet.ID), detatch));
            });

            overlay.SubText = "Success!";
            Schedule(() => panels.Content?.Hide());
        }
        catch (Exception e)
        {
            Logger.Error(e, "An error occurred while uploading the mapset!");
            Schedule(() => panels.Replace(new SingleButtonPanel(
                Phosphor.Bold.Warning,
                $"Failed up {(isUpdate ? "update" : "upload")} mapset!",
                e.Message
            )));
        }
    }
}
