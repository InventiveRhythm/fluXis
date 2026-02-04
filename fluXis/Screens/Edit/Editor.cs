using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Requests.Maps;
using fluXis.Online.API.Requests.MapSets;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Overlay.Wiki;
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
using fluXis.Screens.Edit.UI.TabSwitcher;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Scripting;
using fluXis.Skinning.Default;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using ManagedBass.Fx;
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

    /// <summary>
    /// overwrites the tab the editor opens with
    /// </summary>
    public int StartTabIndex { get; init; } = -1;

    private ITrackStore trackStore { get; set; }

    private EditorLoader loader { get; }

    private Container<EditorTab> tabs;
    private int currentTab;

    public Bindable<Waveform> Waveform { get; private set; }
    private EditorMap editorMap { get; }

    private EditorClock clock;
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

    private bool canSave => editorMap.RealmMap.StatusInt < 100;

    public bool HasUnsavedChanges
    {
        get
        {
            if (!canSave)
                return false;

            return editorMap.MapEventsHash != lastEffectHash
                   || editorMap.MapInfoHash != lastMapHash
                   || editorMap.StoryboardHash != lastStoryboardHash;
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

    public Editor(EditorLoader loader, RealmMap realmMap = null, EditorMap.EditorMapInfo map = null)
    {
        this.loader = loader;
        editorMap = new EditorMap(map, realmMap, LoadComponent, Scheduler);
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, GameHost host, FluXisConfig config, ExperimentConfigManager experiments)
    {
        BindableBackgroundDim = config.GetBindable<float>(FluXisSetting.EditorDim);
        BindableBackgroundBlur = config.GetBindable<float>(FluXisSetting.EditorBlur);

        globalClock.Looping = false;

        editorMap.Panels = panels;
        isNewMap = editorMap.IsNew;

        if (editorMap.RealmMap == null)
        {
            editorMap.RealmMap = mapStore.CreateNew();
        }
        else
        {
            var resources = editorMap.RealmMap.MapSet.Resources;
            editorMap.RealmMap = editorMap.RealmMap.Detach();
            editorMap.RealmMap.MapSet.Resources = resources;
        }

        editorMap.MapInfo ??= new EditorMap.EditorMapInfo(new MapMetadata { Mapper = editorMap.RealmMap.Metadata.Mapper }) { NewLaneSwitchLayout = true, RealmEntry = editorMap.RealmMap };
        editorMap.MapInfo.MapEvents ??= new MapEvents();
        editorMap.MapInfo.Storyboard ??= new Storyboard();

        editorMap.SetupWatcher();
        editorMap.SetupNotifiers();

        backgrounds.AddBackgroundFromMap(editorMap.RealmMap);
        trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(host.Storage.GetStorageForDirectory("maps")));

        dependencies.CacheAs(this);
        dependencies.CacheAs(editorMap);
        dependencies.CacheAs<ICustomColorProvider>(editorMap.MapInfo.Colors);
        dependencies.CacheAs(Waveform = new Bindable<Waveform>());
        dependencies.CacheAs(actionStack = new EditorActionStack(editorMap) { NotificationManager = notifications });
        dependencies.CacheAs(modding = new EditorModding());
        dependencies.CacheAs(settings = new EditorSettings
        {
            ShowSamples = config.GetBindable<bool>(FluXisSetting.EditorShowSamples)
        });

        updateStateHash();

        clock = new EditorClock(editorMap.MapInfo) { SnapDivisor = settings.SnapDivisorBindable };
        clock.ChangeSource(loadMapTrack());
        dependencies.CacheAs(clock);
        dependencies.CacheAs<IBeatSyncProvider>(clock);

        dependencies.CacheAs(new EditorSnapProvider(editorMap, settings, clock));

        MapSetPath = MapFiles.GetFullPath($"{editorMap.MapSet.ID}");

        if (!Directory.Exists(MapSetPath))
            Directory.CreateDirectory(MapSetPath);

        var scripts = new ScriptStorage(MapSetPath);
        editorMap.ScriptChanged += _ =>
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

        verifyTab = tabList.OfType<VerifyTab>().First();

        keybinds = new EditorKeybindingContainer(this, config.GetBindable<string>(FluXisSetting.EditorKeymap), host);
        dependencies.CacheAs(keybinds);

        InternalChild = keybinds.WithChildren(new Drawable[]
        {
            lowPass = new AudioFilter(audioManager.TrackMixer),
            highPass = new AudioFilter(audioManager.TrackMixer, BQFType.HighPass),
            clock,
            modding,
            dependencies.CacheAsAndReturn(new Hitsounding(editorMap.RealmMap.MapSet, editorMap.MapInfo.HitSoundFades, clock.RateBindable)
            {
                DirectVolume = true,
                Clock = clock
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
                                new MenuExpandItem("File", FontAwesome6.Solid.File, new FluXisMenuItem[]
                                {
                                    new MenuActionItem("Save", FontAwesome6.Solid.FloppyDisk, () => save()) { IsEnabled = () => HasUnsavedChanges },
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Create new difficulty...", FontAwesome6.Solid.Plus, () => panels.Content = new EditorDifficultyCreationPanel
                                    {
                                        OnCreate = createNewDiff
                                    }) { IsEnabled = () => canSave },
                                    new MenuActionItem("Switch to difficulty", FontAwesome6.Solid.RightLeft, () => { })
                                    {
                                        IsEnabled = () => editorMap.MapSet.Maps.Count > 1,
                                        Items = editorMap.MapSet.Maps.Where(x => x.ID != editorMap.RealmMap.ID)
                                                         .Select(x => new MenuActionItem(x.Difficulty, FontAwesome6.Solid.Circle, () => loader.SwitchTo(x))).ToList()
                                    },
                                    new MenuActionItem("Delete difficulty...", FontAwesome6.Solid.Trash, () =>
                                    {
                                        panels.Content = new ConfirmDeletionPanel(() =>
                                        {
                                            // delete diff
                                            mapStore.DeleteDifficulty(editorMap.MapSet, editorMap.RealmMap);

                                            // requery mapset
                                            var set = mapStore.GetFromGuid(editorMap.MapSet.ID);

                                            // switch to other diff
                                            var other = set.Maps.FirstOrDefault(x => x.ID != editorMap.RealmMap.ID);
                                            loader.SwitchTo(other);
                                        }, itemName: "difficulty");
                                    })
                                    {
                                        IsEnabled = () => editorMap.MapSet.Maps.Count > 1 && canSave
                                    },
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Export", FontAwesome6.Solid.BoxOpen, export),
                                    new MenuActionItem("Upload...", FontAwesome6.Solid.Upload, startUpload) { IsEnabled = () => canSave && api.IsLoggedIn },
                                    new MenuActionItem("Submit to Queue...", FontAwesome6.Solid.Upload, submitToQueue) { IsEnabled = () => editorMap.MapSet.OnlineID > 0 && api.IsLoggedIn },
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Open Song Folder", FontAwesome6.Solid.FolderOpen, openFolder),
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Exit", FontAwesome6.Solid.DoorOpen, MenuItemType.Dangerous, tryExit)
                                }),
                                new MenuExpandItem("Edit", FontAwesome6.Solid.Pen, new FluXisMenuItem[]
                                {
                                    new MenuActionItem("Undo", FontAwesome6.Solid.RotateLeft, actionStack.Undo) { IsEnabled = () => actionStack.CanUndo },
                                    new MenuActionItem("Redo", FontAwesome6.Solid.RotateRight, actionStack.Redo) { IsEnabled = () => actionStack.CanRedo },
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Copy", FontAwesome6.Solid.Copy, () => ChartingContainer?.Copy())
                                        { IsEnabled = () => ChartingContainer?.BlueprintContainer.SelectionHandler.SelectedObjects.Any() ?? false },
                                    new MenuActionItem("Cut", FontAwesome6.Solid.Cut, () => ChartingContainer?.Copy(true))
                                        { IsEnabled = () => ChartingContainer?.BlueprintContainer.SelectionHandler.SelectedObjects.Any() ?? false },
                                    new MenuActionItem("Paste", FontAwesome6.Solid.Paste, () => ChartingContainer?.Paste()),
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Apply Offset...", FontAwesome6.Solid.Clock, applyOffset),
                                    new MenuActionItem("Flip Selection", FontAwesome6.Solid.LeftRight, () => ChartingContainer?.FlipSelection())
                                        { IsEnabled = () => ChartingContainer?.CanFlipSelection ?? false },
                                    new MenuActionItem("Shuffle Selection", FontAwesome6.Solid.Shuffle, () => ChartingContainer?.ShuffleSelection())
                                        { IsEnabled = () => ChartingContainer?.CanShuffleSelection ?? false },
                                    new MenuActionItem("Re-snap all notes", FontAwesome6.Solid.ArrowsRotate, () => ChartingContainer?.ReSnapAll()),
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Select all", FontAwesome6.Solid.ObjectGroup, () => ChartingContainer?.BlueprintContainer.SelectAll()),
                                    new MenuActionItem("Delete", FontAwesome6.Solid.Trash, () => ChartingContainer?.BlueprintContainer.SelectionHandler.DeleteSelected()),
                                    new MenuSpacerItem(),
                                    new MenuActionItem("Editor Keymap...", FontAwesome6.Solid.Keyboard, () => keymapOverlay.Show())
                                }),
                                new MenuExpandItem("View", FontAwesome6.Solid.Eye, createView()),
                                new MenuExpandItem("Timing", FontAwesome6.Solid.Clock, new FluXisMenuItem[]
                                {
                                    new MenuActionItem("Set preview point to current time", FontAwesome6.Solid.Stopwatch, () =>
                                    {
                                        editorMap.MapInfo.Metadata.PreviewTime
                                            = editorMap.RealmMap.Metadata.PreviewTime
                                                = (int)clock.CurrentTime;
                                    })
                                }),
                                new MenuExpandItem("Audio", FontAwesome6.Solid.VolumeHigh, new FluXisMenuItem[]
                                {
                                    new MenuToggleItem("Enable Low Pass filter", FontAwesome6.Solid.AngleDown, () =>
                                    {
                                        lowPassEnabled = !lowPassEnabled;
                                        lowPass.CutoffTo(lowPassEnabled ? AudioFilter.MIN : AudioFilter.MAX, 400);
                                    }, () => lowPassEnabled),
                                    new MenuToggleItem("Enable High Pass filter", FontAwesome6.Solid.AngleUp, () =>
                                    {
                                        highPassEnabled = !highPassEnabled;
                                        highPass.CutoffTo(highPassEnabled ? 300 : 0, 400);
                                    }, () => highPassEnabled)
                                }),
                                new MenuActionItem("Wiki", FontAwesome6.Solid.Book, openHelp)
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
                new MenuExpandItem("Background Dim", FontAwesome6.Solid.Image, createPercentItems(() => BindableBackgroundDim.Value, v => BindableBackgroundDim.Value = v)),
                new MenuExpandItem("Background Blur", FontAwesome6.Solid.Aperture, createPercentItems(() => BindableBackgroundBlur.Value, v => BindableBackgroundBlur.Value = v)),
                new MenuSpacerItem(),
                new MenuExpandItem("Waveform opacity", FontAwesome6.Solid.WaveformLines, createPercentItems(() => settings.WaveformOpacity.Value, v => settings.WaveformOpacity.Value = v)),
                new MenuSpacerItem(),
                new MenuToggleItem("Show sample on notes", FontAwesome6.Solid.LayerGroup, settings.ShowSamples),
                new MenuSpacerItem(),
                new MenuToggleItem("Force 16:9 Ratio", FontAwesome6.Solid.RectangleWide, settings.ForceAspectRatio),
                new MenuToggleItem("Compact Sidebar", FontAwesome6.Solid.ArrowsToLine, config.GetBindable<bool>(FluXisSetting.EditorCompactMode)),
            };

            if (experiments.Get<bool>(ExperimentConfig.ModView))
                list.Add(new MenuToggleItem("Toggle ModView", FontAwesome6.Solid.Pen, () => modding.Toggle(), () => modding.IsActive));

            return list.ToArray();
        }
    }

    private FluXisMenuItem[] createPercentItems(Func<float> get, Action<float> set)
    {
        float[] values = { 0, .2f, .4f, .6f, .8f, 1 };
        return values.Select(x => new MenuToggleItem($"{x * 100:0}%", FontAwesome6.Solid.Percent, () => set(x), () => Math.Abs(get() - x) < .01f)).ToArray<FluXisMenuItem>();
    }

    private void updateStateHash()
    {
        lastMapHash = editorMap.MapInfoHash;
        lastEffectHash = editorMap.MapEventsHash;
        lastStoryboardHash = editorMap.StoryboardHash;
    }

    private void applyOffset()
    {
        panels.Content = new EditorOffsetPanel
        {
            OnApplyOffset = offset => actionStack.Add(new ApplyOffsetAction(offset))
        };
    }

    private void createNewDiff(CreateNewMapParameters param)
    {
        if (diffExists(param.DifficultyName))
            return;

        panels.Content.Hide();
        loader.CreateNewDifficulty(editorMap.RealmMap, editorMap.MapInfo, param);

        bool diffExists(string name)
        {
            if (!editorMap.MapSet.Maps.Any(x => string.Equals(x.Difficulty, name, StringComparison.CurrentCultureIgnoreCase)))
                return false;

            notifications.SendError("A difficulty with that name already exists!");
            return true;
        }
    }

    private Track loadMapTrack()
    {
        string path = editorMap.MapSet?.GetPathForFile(editorMap.RealmMap.Metadata?.Audio);

        Waveform w = null;

        if (!string.IsNullOrEmpty(path))
        {
            Stream s = trackStore.GetStream(path);
            if (s != null) w = new Waveform(s);
        }

        Waveform.Value = w;
        return editorMap.RealmMap.GetTrack() ?? trackStore.GetVirtual(10000);
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
                FontAwesome6.Solid.ExclamationTriangle,
                "This map is from another game!",
                "You can edit and playtest, but not save or upload.");
        }

        BindableBackgroundDim.BindValueChanged(updateDim, true);
        BindableBackgroundBlur.BindValueChanged(updateBlur, true);

        editorMap.AudioChanged += () => clock.ChangeSource(loadMapTrack());
        editorMap.BackgroundChanged += () => backgrounds.AddBackgroundFromMap(editorMap.RealmMap);

        editorMap.ScriptWatcher.Enable();
    }

    protected override void Update()
    {
        base.Update();

        // too lazy to properly do this
        settings.InvertedScroll.Value = keybinds.Keymap.InvertScroll;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        clock.Stop();
        BindableBackgroundDim.UnbindAll();
        BindableBackgroundBlur.UnbindAll();
        editorMap.ScriptWatcher.Dispose();
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

    private void openFolder() => MapFiles.PresentExternally(editorMap.RealmMap);

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
            var accurate = clock.CurrentTimeAccurate;

            var note = direction > 0
                ? editorMap.MapEvents.NoteEvents.FirstOrDefault(n => n.Time > accurate)
                : editorMap.MapEvents.NoteEvents.LastOrDefault(n => n.Time < accurate);

            if (note is not null)
                clock.SeekSmoothly(note.Time);
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
            mapStore.DeleteMapSet(editorMap.MapSet);

        // I hate this but it works. I hate this but it works. I hate this but it works.
        this.Delay(EditorLoader.DURATION * .98f).FadeOut();

        lowPass.CutoffTo(AudioFilter.MAX, 400);
        highPass.CutoffTo(0, 400);
        clock.Track.Value.VolumeTo(0, EditorLoader.DURATION);
        globalClock.Seek((float)clock.CurrentTime);
        panels.Content?.Hide();
        setSystemCursorVisibility(false);
        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
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
        if (editorMap.RealmMap.Metadata.Mapper == api.User.Value?.Username)
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

        if (editorMap.MapInfo.TimingPoints.Count == 0)
        {
            notifications.SendError("Map has no timing points!");
            return false;
        }

        editorMap.Sort();

        if (!HasUnsavedChanges)
        {
            notifications.SendSmallText("Map is already up to date", FontAwesome6.Solid.Check);
            return true;
        }

        editorMap.ScriptWatcher.Disable();

        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        editorMap.MapInfo.TimeInEditor += now - lastSaveTime;

        mapStore.Save(editorMap.RealmMap, editorMap.MapInfo, editorMap.MapEvents, editorMap.Storyboard, setStatus);
        Scheduler.ScheduleOnceIfNeeded(() => mapStore.UpdateMapSet(mapStore.GetFromGuid(editorMap.MapSet.ID), editorMap.MapSet));

        isNewMap = false;
        updateStateHash();
        notifications.SendSmallText("Saved!", FontAwesome6.Solid.Check);
        lastSaveTime = now;

        editorMap.ScriptWatcher.Enable();
        return true;
    }

    private void export()
    {
        if (!save(false)) return;

        mapStore.Export(editorMap.MapSet, new TaskNotificationData
        {
            Text = $"{editorMap.MapInfo.Metadata.Title} - {editorMap.MapInfo.Metadata.Artist}",
            TextWorking = "Exporting..."
        });
    }

    private void tryExit() => this.Exit(); // TODO: unsaved changes check

    private void submitToQueue()
    {
        if (editorMap.MapSet.OnlineID <= 0)
            return;

        var panel = new EditorUploadOverlay
        {
            Text = "Submitting to queue...",
            SubText = "Please wait..."
        };

        panels.Content = panel;

        var req = new MapSetSubmitQueueRequest(editorMap.MapSet.OnlineID);
        req.Success += _ =>
        {
            notifications.SendSmallText("Submitted to queue!");
            panel.Hide();
        };
        req.Failure += ex => panels.Replace(new SingleButtonPanel(
            FontAwesome6.Solid.ExclamationTriangle,
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

        var isUpdate = editorMap.MapSet.OnlineID > 0;

        if (isUpdate)
        {
            panels.Content = new ButtonPanel
            {
                Icon = FontAwesome6.Solid.ExclamationTriangle,
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
                Title = editorMap.MapInfo.Metadata.Title,
                Artist = editorMap.MapInfo.Metadata.Artist
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
                    Icon = FontAwesome6.Solid.ExclamationTriangle,
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

            foreach (var map in editorMap.MapSet.Maps)
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
                    FontAwesome6.Solid.ExclamationTriangle,
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

            var realmMapSet = mapStore.GetFromGuid(editorMap.MapSet.ID);
            var path = mapStore.Export(realmMapSet.Detach(), new TaskNotificationData(), false);
            var buffer = await File.ReadAllBytesAsync(path);

            overlay.SubText = "0%";

            if (setID == -1 && isUpdate)
                setID = editorMap.MapSet.OnlineID;

            var request = new MapSetUploadRequest(buffer, setID);
            request.Progress += (l1, l2) => overlay.SubText = $"{StringUtils.FormatBytes(l1)}/{StringUtils.FormatBytes(l2)} {Math.Round((float)l1 / l2 * 100, 2).ToStringInvariant("00.00")}%";
            await api.PerformRequestAsync(request);

            overlay.SubText = "Reading response...";

            if (!request.IsSuccessful)
            {
                Schedule(() => panels.Replace(new SingleButtonPanel(
                    FontAwesome6.Solid.ExclamationTriangle,
                    $"Failed up {(isUpdate ? "update" : "upload")} mapset!",
                    request.FailReason?.Message ?? APIRequest.UNKNOWN_ERROR
                )));
                return;
            }

            overlay.SubText = "Assigning IDs...";

            realm.RunWrite(r =>
            {
                var set = r.Find<RealmMapSet>(editorMap.MapSet.ID);
                set.OnlineID = editorMap.MapSet.OnlineID = request.Response!.Data.ID;
                set.SetStatus(request.Response.Data.Status);
                editorMap.MapSet.SetStatus(request.Response.Data.Status);

                foreach (var onlineMap in request.Response.Data.Maps)
                {
                    var map = set.Maps.First(m => m.FileName == onlineMap.FileName);
                    var loadedMap = editorMap.MapSet.Maps.First(m => m.FileName == onlineMap.FileName);

                    map.OnlineID = loadedMap.OnlineID = onlineMap.ID;
                }

                var detatch = set.Detach();
                Schedule(() => mapStore.UpdateMapSet(mapStore.GetFromGuid(editorMap.MapSet.ID), detatch));
            });

            overlay.SubText = "Success!";
            Schedule(() => panels.Content?.Hide());
        }
        catch (Exception e)
        {
            Logger.Error(e, "An error occurred while uploading the mapset!");
            Schedule(() => panels.Replace(new SingleButtonPanel(
                FontAwesome6.Solid.ExclamationTriangle,
                $"Failed up {(isUpdate ? "update" : "upload")} mapset!",
                e.Message
            )));
        }
    }
}
