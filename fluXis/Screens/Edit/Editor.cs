using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Configuration.Experiments;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Graphics.Background;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Context;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Graphics.UserInterface.Text;
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
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.BottomBar;
using fluXis.Screens.Edit.Input;
using fluXis.Screens.Edit.MenuBar;
using fluXis.Screens.Edit.Tabs;
using fluXis.Screens.Edit.Tabs.Charting;
using fluXis.Screens.Edit.Tabs.Storyboarding;
using fluXis.Screens.Edit.TabSwitcher;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Extensions;
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

    /// <summary>
    /// overwrites the tab the editor opens with
    /// </summary>
    public int StartTabIndex { get; init; } = -1;

    private ITrackStore trackStore { get; set; }

    private EditorLoader loader { get; }

    private Container<EditorTab> tabs;
    private int currentTab;

    private EditorMenuBar menuBar;
    private EditorTabSwitcher tabSwitcher;
    private EditorBottomBar bottomBar;

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

    public ChartingContainer ChartingContainer { get; set; }

    public Editor(EditorLoader loader, RealmMap realmMap = null, EditorMap.EditorMapInfo map = null)
    {
        this.loader = loader;

        editorMap = new EditorMap
        {
            RealmMap = realmMap,
            MapInfo = map
        };
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, Storage storage, FluXisConfig config, ExperimentConfigManager experiments)
    {
        BindableBackgroundDim = config.GetBindable<float>(FluXisSetting.EditorDim);
        BindableBackgroundBlur = config.GetBindable<float>(FluXisSetting.EditorBlur);

        globalClock.Looping = false;

        editorMap.Panels = panels;
        isNewMap = editorMap.IsNew;

        if (editorMap.RealmMap == null)
            editorMap.RealmMap = mapStore.CreateNew();
        else
        {
            var resources = editorMap.RealmMap.MapSet.Resources;
            editorMap.RealmMap = editorMap.RealmMap.Detach();
            editorMap.RealmMap.MapSet.Resources = resources;
        }

        editorMap.MapInfo ??= new EditorMap.EditorMapInfo(new MapMetadata { Mapper = editorMap.RealmMap.Metadata.Mapper }) { NewLaneSwitchLayout = true, RealmEntry = editorMap.RealmMap };
        editorMap.MapInfo.MapEvents ??= new MapEvents();
        editorMap.MapInfo.Storyboard ??= new Storyboard();

        editorMap.SetupNotifiers();

        backgrounds.AddBackgroundFromMap(editorMap.RealmMap);
        trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("maps")));

        dependencies.CacheAs(this);
        dependencies.CacheAs(editorMap);
        dependencies.CacheAs(Waveform = new Bindable<Waveform>());
        dependencies.CacheAs(actionStack = new EditorActionStack(editorMap) { NotificationManager = notifications });
        dependencies.CacheAs(settings = new EditorSettings
        {
            ShowSamples = config.GetBindable<bool>(FluXisSetting.EditorShowSamples),
            InvertedScroll = config.GetBindable<bool>(FluXisSetting.InvertScroll)
        });

        updateStateHash();

        clock = new EditorClock(editorMap.MapInfo) { SnapDivisor = settings.SnapDivisorBindable };
        clock.ChangeSource(loadMapTrack());
        dependencies.CacheAs(clock);
        dependencies.CacheAs<IBeatSyncProvider>(clock);

        dependencies.CacheAs(new EditorSnapProvider(editorMap, settings, clock));

        InternalChild = new EditorKeybindingContainer(this, realm)
        {
            Children = new Drawable[]
            {
                lowPass = new AudioFilter(audioManager.TrackMixer),
                highPass = new AudioFilter(audioManager.TrackMixer, BQFType.HighPass),
                clock,
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
                                    Children = new EditorTab[]
                                    {
                                        new SetupTab(),
                                        new ChartingTab(),
                                        new DesignTab(),
                                        experiments.Get<bool>(ExperimentConfig.StoryboardTab) ? new StoryboardTab() : new WipEditorTab(FontAwesome6.Solid.PaintBrush, "Storyboard", "Soon you'll be able to create storyboards here."),
                                        new WipEditorTab(FontAwesome6.Solid.Music, "Hitsounding", "Soon you'll be able to edit volume of hitsounds and other stuff here.")
                                    }
                                }
                            },
                            menuBar = new EditorMenuBar
                            {
                                Items = new FluXisMenuItem[]
                                {
                                    new("File", FontAwesome6.Solid.File)
                                    {
                                        Items = new FluXisMenuItem[]
                                        {
                                            new("Save", FontAwesome6.Solid.FloppyDisk, () => save()) { Enabled = () => HasUnsavedChanges },
                                            new FluXisMenuSpacer(),
                                            new("Create new difficulty", FontAwesome6.Solid.Plus, () => panels.Content = new EditorDifficultyCreationPanel
                                            {
                                                OnCreate = createNewDiff
                                            }) { Enabled = () => canSave },
                                            new("Switch to difficulty", FontAwesome6.Solid.RightLeft, () => { })
                                            {
                                                Enabled = () => editorMap.MapSet.Maps.Count > 1,
                                                Items = editorMap.MapSet.Maps.Where(x => x.ID != editorMap.RealmMap.ID)
                                                                 .Select(x => new FluXisMenuItem(x.Difficulty, () => loader.SwitchTo(x))).ToList()
                                            },
                                            new("Delete difficulty", FontAwesome6.Solid.Trash, () =>
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
                                                Enabled = () => editorMap.MapSet.Maps.Count > 1 && canSave
                                            },
                                            new FluXisMenuSpacer(),
                                            new("Export", FontAwesome6.Solid.BoxOpen, export),
                                            new("Upload", FontAwesome6.Solid.Upload, startUpload) { Enabled = () => canSave && api.IsLoggedIn },
                                            new FluXisMenuSpacer(),
                                            new("Open Song Folder", FontAwesome6.Solid.FolderOpen, openFolder),
                                            new FluXisMenuSpacer(),
                                            new("Exit", FontAwesome6.Solid.DoorOpen, MenuItemType.Dangerous, tryExit)
                                        }
                                    },
                                    new("Edit", FontAwesome6.Solid.Pen)
                                    {
                                        Items = new FluXisMenuItem[]
                                        {
                                            new("Undo", FontAwesome6.Solid.RotateLeft, actionStack.Undo) { Enabled = () => actionStack.CanUndo },
                                            new("Redo", FontAwesome6.Solid.RotateRight, actionStack.Redo) { Enabled = () => actionStack.CanRedo },
                                            new FluXisMenuSpacer(),
                                            new("Copy", FontAwesome6.Solid.Copy, () => ChartingContainer?.Copy())
                                                { Enabled = () => ChartingContainer?.BlueprintContainer.SelectionHandler.SelectedObjects.Any() ?? false },
                                            new("Cut", FontAwesome6.Solid.Cut, () => ChartingContainer?.Copy(true))
                                                { Enabled = () => ChartingContainer?.BlueprintContainer.SelectionHandler.SelectedObjects.Any() ?? false },
                                            new("Paste", FontAwesome6.Solid.Paste, () => ChartingContainer?.Paste()),
                                            new FluXisMenuSpacer(),
                                            new("Apply Offset", FontAwesome6.Solid.Clock, applyOffset),
                                            new("Flip Selection", FontAwesome6.Solid.LeftRight, () => ChartingContainer?.FlipSelection()) { Enabled = () => ChartingContainer?.CanFlipSelection ?? false },
                                            new("Shuffle Selection", FontAwesome6.Solid.Shuffle, () => ChartingContainer?.ShuffleSelection()) { Enabled = () => ChartingContainer?.CanShuffleSelection ?? false },
                                            new("Re-snap all notes", FontAwesome6.Solid.ArrowsRotate, () => ChartingContainer?.ReSnapAll()),
                                            new FluXisMenuSpacer(),
                                            new("Delete", FontAwesome6.Solid.Trash, () => ChartingContainer?.BlueprintContainer.SelectionHandler.DeleteSelected()),
                                            new("Select all", FontAwesome6.Solid.ObjectGroup, () => ChartingContainer?.BlueprintContainer.SelectAll())
                                        }
                                    },
                                    new("View", FontAwesome6.Solid.Eye)
                                    {
                                        Items = new FluXisMenuItem[]
                                        {
                                            new("Background Dim", FontAwesome6.Solid.Image)
                                            {
                                                Items = new FluXisMenuItem[]
                                                {
                                                    new("0%", FontAwesome6.Solid.Percent, () => BindableBackgroundDim.Value = 0) { IsActive = () => BindableBackgroundDim.Value == 0f },
                                                    new("20%", FontAwesome6.Solid.Percent, () => BindableBackgroundDim.Value = .2f) { IsActive = () => BindableBackgroundDim.Value == .2f },
                                                    new("40%", FontAwesome6.Solid.Percent, () => BindableBackgroundDim.Value = .4f) { IsActive = () => BindableBackgroundDim.Value == .4f },
                                                    new("60%", FontAwesome6.Solid.Percent, () => BindableBackgroundDim.Value = .6f) { IsActive = () => BindableBackgroundDim.Value == .6f },
                                                    new("80%", FontAwesome6.Solid.Percent, () => BindableBackgroundDim.Value = .8f) { IsActive = () => BindableBackgroundDim.Value == .8f },
                                                }
                                            },
                                            new("Background Blur", FontAwesome6.Solid.Aperture)
                                            {
                                                Items = new FluXisMenuItem[]
                                                {
                                                    new("0%", FontAwesome6.Solid.Percent, () => BindableBackgroundBlur.Value = 0) { IsActive = () => BindableBackgroundBlur.Value == 0f },
                                                    new("20%", FontAwesome6.Solid.Percent, () => BindableBackgroundBlur.Value = .2f) { IsActive = () => BindableBackgroundBlur.Value == .2f },
                                                    new("40%", FontAwesome6.Solid.Percent, () => BindableBackgroundBlur.Value = .4f) { IsActive = () => BindableBackgroundBlur.Value == .4f },
                                                    new("60%", FontAwesome6.Solid.Percent, () => BindableBackgroundBlur.Value = .6f) { IsActive = () => BindableBackgroundBlur.Value == .6f },
                                                    new("80%", FontAwesome6.Solid.Percent, () => BindableBackgroundBlur.Value = .8f) { IsActive = () => BindableBackgroundBlur.Value == .8f },
                                                    new("100%", FontAwesome6.Solid.Percent, () => BindableBackgroundBlur.Value = 1f) { IsActive = () => BindableBackgroundBlur.Value == 1f }
                                                }
                                            },
                                            new FluXisMenuSpacer(),
                                            new("Waveform opacity", FontAwesome6.Solid.WaveformLines)
                                            {
                                                Items = new FluXisMenuItem[]
                                                {
                                                    new("0%", FontAwesome6.Solid.Percent, () => settings.WaveformOpacity.Value = 0) { IsActive = () => settings.WaveformOpacity.Value == 0 },
                                                    new("25%", FontAwesome6.Solid.Percent, () => settings.WaveformOpacity.Value = 0.25f) { IsActive = () => settings.WaveformOpacity.Value == 0.25f },
                                                    new("50%", FontAwesome6.Solid.Percent, () => settings.WaveformOpacity.Value = 0.5f) { IsActive = () => settings.WaveformOpacity.Value == 0.5f },
                                                    new("75%", FontAwesome6.Solid.Percent, () => settings.WaveformOpacity.Value = 0.75f) { IsActive = () => settings.WaveformOpacity.Value == 0.75f },
                                                    new("100%", FontAwesome6.Solid.Percent, () => settings.WaveformOpacity.Value = 1) { IsActive = () => settings.WaveformOpacity.Value == 1 }
                                                }
                                            },
                                            new FluXisMenuSpacer(),
                                            new("Flash underlay", FontAwesome6.Solid.LayerGroup, settings.FlashUnderlay.Toggle) { IsActive = () => settings.FlashUnderlay.Value },
                                            new("Underlay color", FontAwesome6.Solid.Palette)
                                            {
                                                Items = new FluXisMenuItem[]
                                                {
                                                    new("Dark", () => settings.FlashUnderlayColor.Value = FluXisColors.Background1)
                                                        { IsActive = () => settings.FlashUnderlayColor.Value == FluXisColors.Background1 },
                                                    new("Light", () => settings.FlashUnderlayColor.Value = Colour4.White) { IsActive = () => settings.FlashUnderlayColor.Value == Colour4.White }
                                                }
                                            },
                                            new FluXisMenuSpacer(),
                                            new("Show sample on notes", FontAwesome6.Solid.LayerGroup, () => settings.ShowSamples.Value = !settings.ShowSamples.Value)
                                                { IsActive = () => settings.ShowSamples.Value },
                                            new FluXisMenuSpacer(),
                                            new("Invert scroll direction", FontAwesome6.Solid.UpDown, () => settings.InvertedScroll.Value = !settings.InvertedScroll.Value)
                                                { IsActive = () => settings.InvertedScroll.Value },
                                            new FluXisMenuSpacer(),
                                            new("Force 16:9 Ratio", FontAwesome6.Solid.RectangleWide, () => settings.ForceAspectRatio.Toggle())
                                                { IsActive = () => settings.ForceAspectRatio.Value },
                                        }
                                    },
                                    new("Timing", FontAwesome6.Solid.Clock)
                                    {
                                        Items = new FluXisMenuItem[]
                                        {
                                            new("Set preview point to current time", FontAwesome6.Solid.Stopwatch, () =>
                                            {
                                                editorMap.MapInfo.Metadata.PreviewTime
                                                    = editorMap.RealmMap.Metadata.PreviewTime
                                                        = (int)clock.CurrentTime;
                                            })
                                        }
                                    },
                                    new("Audio", FontAwesome6.Solid.VolumeHigh)
                                    {
                                        Items = new FluXisMenuItem[]
                                        {
                                            new("Enable Low Pass filter", FontAwesome6.Solid.AngleDown, () =>
                                            {
                                                lowPassEnabled = !lowPassEnabled;
                                                lowPass.CutoffTo(lowPassEnabled ? AudioFilter.MIN : AudioFilter.MAX, 400);
                                            }) { IsActive = () => lowPassEnabled },
                                            new("Enable High Pass filter", FontAwesome6.Solid.AngleUp, () =>
                                            {
                                                highPassEnabled = !highPassEnabled;
                                                highPass.CutoffTo(highPassEnabled ? 300 : 0, 400);
                                            }) { IsActive = () => highPassEnabled }
                                        }
                                    },
                                    new("Wiki", FontAwesome6.Solid.Book, openHelp)
                                }
                            },
                            tabSwitcher = new EditorTabSwitcher
                            {
                                ChildrenEnumerable = tabs.Select(x => new EditorTabSwitcherButton(x.Icon, x.TabName, () => changeTab(tabs.IndexOf(x))))
                            },
                            bottomBar = new EditorBottomBar()
                        }
                    }
                }
            }
        };
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
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        clock.Stop();
        BindableBackgroundDim.UnbindAll();
        BindableBackgroundBlur.UnbindAll();
    }

    private void updateDim(ValueChangedEvent<float> e) => backgrounds.SetDim(e.NewValue, 400);
    private void updateBlur(ValueChangedEvent<float> e) => backgrounds.SetBlur(e.NewValue, 400);

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

    private void openHelp() => Game.OpenLink($"{api.Endpoint.WikiRootUrl}/editor");
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
            panels.Content ??= new ButtonPanel
            {
                Icon = FontAwesome6.Solid.ExclamationTriangle,
                Text = "There are unsaved changes.",
                SubText = "Are you sure you want to exit?",
                IsDangerous = true,
                Buttons = new ButtonData[]
                {
                    new PrimaryButtonData("Save and exit.", () =>
                    {
                        if (!save())
                            return;

                        exitConfirmed = true;
                        this.Exit();
                    }),
                    new DangerButtonData("Exit without saving.", () =>
                    {
                        exitConfirmed = true;
                        this.Exit();
                    }),
                    new CancelButtonData("Nevermind, back to editing.")
                }
            };

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
        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        enterAnimation();
        FinishTransforms(true);
    }

    public override void OnResuming(ScreenTransitionEvent e) => enterAnimation();
    public override void OnSuspending(ScreenTransitionEvent e) => exitAnimation();

    private void exitAnimation()
    {
        lowPass.CutoffTo(AudioFilter.MAX, MOVE_DURATION);
        highPass.CutoffTo(0, MOVE_DURATION);
        this.ScaleTo(1.2f, MOVE_DURATION, Easing.OutQuint).FadeOut(FADE_DURATION);
    }

    private void enterAnimation()
    {
        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.ScaleTo(1f).FadeInFromZero(FADE_DURATION);
            lowPass.CutoffTo(lowPassEnabled ? AudioFilter.MIN : AudioFilter.MAX, MOVE_DURATION);
            highPass.CutoffTo(highPassEnabled ? 300 : 0, MOVE_DURATION);
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

        if (editorMap.MapInfo.HitObjects.Count == 0)
        {
            notifications.SendError("Map has no hit objects!");
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

        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        editorMap.MapInfo.TimeInEditor += now - lastSaveTime;

        mapStore.Save(editorMap.RealmMap, editorMap.MapInfo, editorMap.MapEvents, editorMap.Storyboard, setStatus);
        Scheduler.ScheduleOnceIfNeeded(() => mapStore.UpdateMapSet(mapStore.GetFromGuid(editorMap.MapSet.ID), editorMap.MapSet));

        isNewMap = false;
        updateStateHash();
        notifications.SendSmallText("Saved!", FontAwesome6.Solid.Check);
        lastSaveTime = now;
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

    private void startUpload()
    {
        if (isUploading)
            return;

        if (!api.IsLoggedIn)
        {
            notifications.SendError("You need to be logged in to upload maps!");
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
                            t.Colour = FluXisColors.Link;
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
            if (!canSave)
            {
                notifications.SendError("Map is from another game!");
                Schedule(() => panels.Content?.Hide());
                return;
            }

            var overlay = new EditorUploadOverlay
            {
                Text = isUpdate ? "Updating mapset..." : "Uploading mapset...",
                SubText = "Saving..."
            };

            Schedule(() => panels.Content = overlay);

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
            request.Progress += (l1, l2) => overlay.SubText = $"{Math.Round((float)l1 / l2 * 100, 2).ToStringInvariant("00.00")}%";
            await api.PerformRequestAsync(request);

            overlay.SubText = "Reading response...";

            if (!request.IsSuccessful)
            {
                notifications.SendError(request.FailReason?.Message ?? APIRequest.UNKNOWN_ERROR);
                Schedule(() => panels.Content?.Hide());
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
            notifications.SendError("An error occurred while uploading the mapset!", e.Message);
            Logger.Error(e, "An error occurred while uploading the mapset!");
            Schedule(() => panels.Content?.Hide());
        }
    }
}
