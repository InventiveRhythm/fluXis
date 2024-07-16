using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Configuration.Experiments;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Map;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Requests.Maps;
using fluXis.Game.Online.API.Requests.MapSets;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.BottomBar;
using fluXis.Game.Screens.Edit.Input;
using fluXis.Game.Screens.Edit.MenuBar;
using fluXis.Game.Screens.Edit.Tabs;
using fluXis.Game.Screens.Edit.Tabs.Charting;
using fluXis.Game.Screens.Edit.TabSwitcher;
using fluXis.Game.Utils;
using fluXis.Game.Utils.Extensions;
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

namespace fluXis.Game.Screens.Edit;

public partial class Editor : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>, IKeyBindingHandler<EditorKeybinding>
{
    public override bool ShowToolbar => false;
    public override float BackgroundDim => backgroundDim.Value;
    public override float BackgroundBlur => backgroundBlur.Value;
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

    private DependencyContainer dependencies;
    private bool exitConfirmed;
    private bool isNewMap;
    private bool isUploading;

    private string lastMapHash;
    private string lastEffectHash;

    private bool canSave => editorMap.RealmMap.StatusInt < 100;

    public bool HasUnsavedChanges
    {
        get
        {
            if (!canSave)
                return false;

            return editorMap.MapEventsHash != lastEffectHash || editorMap.MapInfoHash != lastMapHash;
        }
    }

    private Bindable<float> backgroundDim;
    private Bindable<float> backgroundBlur;

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
        backgroundDim = config.GetBindable<float>(FluXisSetting.EditorDim);
        backgroundBlur = config.GetBindable<float>(FluXisSetting.EditorBlur);

        globalClock.Looping = false;
        globalClock.Stop(); // the editor clock will handle this

        isNewMap = editorMap.IsNew;

        if (editorMap.RealmMap == null)
            editorMap.RealmMap = mapStore.CreateNew();
        else
        {
            var resources = editorMap.RealmMap.MapSet.Resources;
            editorMap.RealmMap = editorMap.RealmMap.Detach();
            editorMap.RealmMap.MapSet.Resources = resources;
        }

        editorMap.MapInfo ??= new EditorMap.EditorMapInfo(new MapMetadata { Mapper = editorMap.RealmMap.Metadata.Mapper });
        editorMap.MapInfo.MapEvents ??= new MapEvents();

        backgrounds.AddBackgroundFromMap(editorMap.RealmMap);
        trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("maps")));

        dependencies.CacheAs(this);
        dependencies.CacheAs(editorMap);
        dependencies.CacheAs(Waveform = new Bindable<Waveform>());
        dependencies.CacheAs(actionStack = new EditorActionStack(editorMap) { NotificationManager = notifications });
        dependencies.CacheAs(settings = new EditorSettings
        {
            ShowSamples = config.GetBindable<bool>(FluXisSetting.EditorShowSamples)
        });

        updateStateHash();

        clock = new EditorClock(editorMap.MapInfo) { SnapDivisor = settings.SnapDivisorBindable };
        clock.ChangeSource(loadMapTrack());
        dependencies.CacheAs(clock);
        dependencies.CacheAs<IBeatSyncProvider>(clock);

        InternalChild = new EditorKeybindingContainer(this, realm)
        {
            Children = new Drawable[]
            {
                clock,
                new FluXisContextMenuContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = 45, Bottom = 60 },
                            Child = new PopoverContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = tabs = new Container<EditorTab>
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Children = new EditorTab[]
                                    {
                                        new SetupTab(),
                                        new ChartingTab(),
                                        experiments.Get<bool>(ExperimentConfig.DesignTab) ? new DesignTab() : new WipEditorTab(FontAwesome6.Solid.Palette, "Design", "Soon you'll be able to edit effects and other stuff here."),
                                        new WipEditorTab(FontAwesome6.Solid.PaintBrush, "Storyboard", "Soon you'll be able to create storyboards here."),
                                        new WipEditorTab(FontAwesome6.Solid.Music, "Hitsounding", "Soon you'll be able to edit volume of hitsounds and other stuff here.")
                                    }
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
                                            OnCreate = param => createNewDiff(param)
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
                                                mapStore.DeleteDifficultyFromMapSet(editorMap.MapSet, editorMap.RealmMap);

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
                                        new("Exit", FontAwesome6.Solid.XMark, MenuItemType.Dangerous, tryExit)
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
                                        new("Background Dim", FontAwesome6.Solid.Percent)
                                        {
                                            Items = new FluXisMenuItem[]
                                            {
                                                new("0%", FontAwesome6.Solid.Percent, () => backgroundDim.Value = 0) { IsActive = () => backgroundDim.Value == 0f },
                                                new("20%", FontAwesome6.Solid.Percent, () => backgroundDim.Value = .2f) { IsActive = () => backgroundDim.Value == .2f },
                                                new("40%", FontAwesome6.Solid.Percent, () => backgroundDim.Value = .4f) { IsActive = () => backgroundDim.Value == .4f },
                                                new("60%", FontAwesome6.Solid.Percent, () => backgroundDim.Value = .6f) { IsActive = () => backgroundDim.Value == .6f },
                                                new("80%", FontAwesome6.Solid.Percent, () => backgroundDim.Value = .8f) { IsActive = () => backgroundDim.Value == .8f },
                                            }
                                        },
                                        new("Background Blur", FontAwesome6.Solid.Percent)
                                        {
                                            Items = new FluXisMenuItem[]
                                            {
                                                new("0%", FontAwesome6.Solid.Percent, () => backgroundBlur.Value = 0) { IsActive = () => backgroundBlur.Value == 0f },
                                                new("20%", FontAwesome6.Solid.Percent, () => backgroundBlur.Value = .2f) { IsActive = () => backgroundBlur.Value == .2f },
                                                new("40%", FontAwesome6.Solid.Percent, () => backgroundBlur.Value = .4f) { IsActive = () => backgroundBlur.Value == .4f },
                                                new("60%", FontAwesome6.Solid.Percent, () => backgroundBlur.Value = .6f) { IsActive = () => backgroundBlur.Value == .6f },
                                                new("80%", FontAwesome6.Solid.Percent, () => backgroundBlur.Value = .8f) { IsActive = () => backgroundBlur.Value == .8f },
                                                new("100%", FontAwesome6.Solid.Percent, () => backgroundBlur.Value = 1f) { IsActive = () => backgroundBlur.Value == 1f }
                                            }
                                        },
                                        new FluXisMenuSpacer(),
                                        new("Waveform opacity", FontAwesome6.Solid.Percent)
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
                                            { IsActive = () => settings.ShowSamples.Value }
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
        };
    }

    private void updateStateHash()
    {
        lastMapHash = editorMap.MapInfoHash;
        lastEffectHash = editorMap.MapEventsHash;
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
        changeTab(isNewMap ? 0 : 1);

        if (!canSave)
        {
            panels.Content = new ButtonPanel
            {
                Icon = FontAwesome6.Solid.ExclamationTriangle,
                Text = "This map is from another game!",
                SubText = "You can edit and playtest, but not save or upload.",
                Buttons = new ButtonData[]
                {
                    new CancelButtonData("Okay")
                }
            };
        }

        backgroundDim.BindValueChanged(updateDim, true);
        backgroundBlur.BindValueChanged(updateBlur, true);

        editorMap.AudioChanged += () => clock.ChangeSource(loadMapTrack());
        editorMap.BackgroundChanged += () => backgrounds.AddBackgroundFromMap(editorMap.RealmMap);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        clock.Stop();
        backgroundDim.UnbindAll();
        backgroundBlur.UnbindAll();
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
    private void openFolder() => PathUtils.OpenFolder(MapFiles.GetFullPath(editorMap.MapSet.GetPathForFile("")));

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
        }

        return false;
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

        exitAnimation();
        clock.Stop();
        globalClock.Seek((float)clock.CurrentTime);
        panels.Content?.Hide();
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

        // this check won't work 100% of the time, we need a better way of storing the mappers
        if (editorMap.RealmMap.Metadata.Mapper == api.User.Value?.Username)
            Activity.Value = new UserActivity.Editing();
        else
            Activity.Value = new UserActivity.Modding();
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

        mapStore.Save(editorMap.RealmMap, editorMap.MapInfo, editorMap.MapEvents, setStatus);
        Scheduler.ScheduleOnceIfNeeded(() => mapStore.UpdateMapSet(mapStore.GetFromGuid(editorMap.MapSet.ID), editorMap.MapSet));

        isNewMap = false;
        updateStateHash();
        notifications.SendSmallText("Saved!", FontAwesome6.Solid.Check);
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
