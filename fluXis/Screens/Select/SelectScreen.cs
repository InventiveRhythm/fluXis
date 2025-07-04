using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Audio.Transforms;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Database.Score;
using fluXis.Graphics.Background;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Context;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Input;
using fluXis.Integration;
using fluXis.IO;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Online.Activity;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Screens.Edit;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Select.Footer;
using fluXis.Screens.Select.Info;
using fluXis.Screens.Select.List;
using fluXis.Screens.Select.List.Items;
using fluXis.Screens.Select.Mods;
using fluXis.Screens.Select.Search;
using fluXis.Screens.Select.UI;
using fluXis.UI;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Utils;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Select;

public abstract partial class SelectScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.1f;
    public override float BackgroundBlur => songSelectBlur.Value ? 0.25f : 0;
    public override bool ApplyValuesAfterLoad => true;

    public override UserActivity InitialActivity => new UserActivity.SongSelect();

    [Resolved]
    protected MapStore Maps { get; private set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private LightController lightController { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    [Resolved]
    private ReplayStorage replays { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    protected IEnumerable<IMod> CurrentMods => modsOverlay.SelectedMods;

    private MapList mapList { get; set; }
    private ModsOverlay modsOverlay { get; set; }

    // private BufferedContainer modsBlur { get; set; }

    private SelectMapInfo selectMapInfo;
    private SearchBar searchBar;
    private SearchFilterControls filterControl;
    private SelectFooter footer;

    private Sample menuScroll;
    private Sample randomClick;
    private Sample rewindClick;

    private SelectNoMaps noMapsContainer;
    private SelectLetter letterContainer;
    private LoadingIcon loadingIcon;

    private Bindable<bool> songSelectBlur;
    private Bindable<bool> backgroundVideo;

    private DependencyContainer dependencies;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config)
    {
        menuScroll = samples.Get("UI/scroll");
        randomClick = samples.Get("UI/Select/random");
        rewindClick = samples.Get("UI/Select/rewind");

        songSelectBlur = config.GetBindable<bool>(FluXisSetting.SongSelectBlur);
        backgroundVideo = config.GetBindable<bool>(FluXisSetting.BackgroundVideo);

        groupMode = config.GetBindable<MapUtils.GroupingMode>(FluXisSetting.GroupingMode);
        sortMode = config.GetBindable<MapUtils.SortingMode>(FluXisSetting.SortingMode);
        sortInverse = config.GetBindable<bool>(FluXisSetting.SortingInverse);

        dependencies.CacheAs(this);
        dependencies.CacheAs(Filters = new SearchFilters());
        dependencies.CacheAs(modsOverlay = new ModsOverlay());
        dependencies.CacheAs<ISelectionManager>(mapList = new MapList(sortMode));

        InternalChildren = new Drawable[]
        {
            searchTracker = new IdleTracker(250, UpdateSearch),
            /*modsBlur = new BufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false,
                Children = new Drawable[] { }
            },*/
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientVertical(Colour4.Black.Opacity(.8f), Colour4.Black.Opacity(0))
            },
            new FluXisContextMenuContainer()
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Bottom = 50 },
                Children = new Drawable[]
                {
                    new Container
                    {
                        Width = .5f,
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Padding = new MarginPadding(10) { Top = 140, Bottom = 40 },
                        Child = mapList
                    },
                    filterControl = new SearchFilterControls(),
                    searchBar = new SearchBar(),
                    selectMapInfo = new SelectMapInfo
                    {
                        HoverAction = () => mapList.ScrollToSelected()
                    },
                    new Container
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.Both,
                        Width = .5f,
                        Children = new Drawable[]
                        {
                            noMapsContainer = new SelectNoMaps(),
                            letterContainer = new SelectLetter(),
                            loadingIcon = new LoadingIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(50)
                            }
                        }
                    }
                }
            },
            modsOverlay,
            footer = CreateFooter()
        };

        // modsOverlay.BackgroundBlur = modsBlur;
        Filters.OnChange += searchTracker.Reset;
    }

    protected override void LoadComplete()
    {
        if (Maps.CurrentMap.Hash == "dummy" && Maps.MapSets.Any())
            Maps.Select(Maps.GetRandom()?.LowestDifficulty, true);

        sortMode.BindValueChanged(setSortingMode);
        groupMode.BindValueChanged(setGroupingMode);

        Task.Run(() =>
        {
            loadMapSets();

            Schedule(() =>
            {
                Maps.MapSetAdded += addMapSet;
                Maps.MapSetUpdated += replaceMapSet;
                Maps.MapSetRemoved += removeMapSet;
                Maps.MapBindable.BindValueChanged(mapBindableChanged, true);

                mapList.FadeIn(500);
                loadingIcon.FadeOut(500);

                mapList.ScrollToSelected(false);
            });
        });
    }

    protected override void Dispose(bool isDisposing)
    {
        Maps.MapSetAdded -= addMapSet;
        Maps.MapSetUpdated -= replaceMapSet;
        Maps.MapSetRemoved -= removeMapSet;
        songSelectBlur.ValueChanged -= updateBackgroundBlur;
        Maps.MapBindable.ValueChanged -= mapBindableChanged;

        Filters.OnChange -= UpdateSearch;

        menuScroll?.Dispose();
        randomClick?.Dispose();
        rewindClick?.Dispose();

        Items.ForEach(i => i.Unbind());
        Items.Clear();

        base.Dispose(isDisposing);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected void ContinueToReplay(RealmMap map, List<IMod> mods, Func<Replay> replayFunc)
    {
        this.Push(new GameplayLoader(map, mods, () =>
        {
            var replay = replayFunc();
            return replay == null ? null : new ReplayGameplayScreen(map, mods, replay) { Scores = selectMapInfo.ScoreList?.CurrentScores.ToList() };
        }));
    }

    #region Creating

    protected virtual SelectFooter CreateFooter() => new()
    {
        BackAction = () =>
        {
            if (modsOverlay.State.Value == Visibility.Visible)
                modsOverlay.Hide();
            else
                this.Exit();
        },
        ModsAction = modsOverlay.ToggleVisibility,
        RewindAction = RewindRandom,
        RandomAction = RandomMap,
        PlayAction = Accept,
        DeleteAction = DeleteMapSet,
        EditAction = EditMap,
        ScoresWiped = () => selectMapInfo.ScoreList?.Refresh()
    };

    #endregion

    #region Actions

    public void Accept()
    {
        if (Maps.CurrentMap == null)
            return;

        UISamples.Select();
        backgrounds.AddBackgroundFromMap(Maps.CurrentMap);
        backgrounds.SwipeAnimation();

        modsOverlay.Hide();

        StartMap(Maps.CurrentMap, modsOverlay.SelectedMods.ToList(), selectMapInfo.ScoreList?.CurrentScores.ToList());
    }

    protected abstract void StartMap(RealmMap map, List<IMod> mods, List<ScoreInfo> scores);

    public void ViewReplay(RealmMap map, RealmScore score)
    {
        if (!this.IsCurrentScreen())
            this.MakeCurrent();

        var scoreMods = score.Mods.Split(' ').ToList();
        scoreMods.RemoveAll(string.IsNullOrEmpty);

        var mods = scoreMods.Select(ModUtils.GetFromAcronym).ToList();
        mods.RemoveAll(m => m == null);

        if (mods.Count != scoreMods.Count)
        {
            Logger.Log($"Some mods were not found ({mods.Count}:{scoreMods.Count})", LoggingTarget.Runtime, LogLevel.Error);
            return;
        }

        ContinueToReplay(map, mods, () =>
        {
            var replay = replays.Get(score.ID);

            if (replay == null)
            {
                Logger.Log($"Replay for score {score.ID} not found!", LoggingTarget.Runtime, LogLevel.Error);
                return null;
            }

            return replay;
        });
    }

    public void EditMap(RealmMap map)
    {
        if (map == null) return;

        Maps.Select(map, true);
        if (Maps.CurrentMap == null) return;

        if (map.MapSet.AutoImported)
        {
            panels.Content = new SingleButtonPanel(
                FontAwesome6.Solid.ExclamationTriangle,
                "This map cannot be edited.",
                "This map is auto-imported from a different game and cannot be opened in the editor.");
            return;
        }

        var loadedMap = map.GetMapInfo();
        if (loadedMap == null) return;

        var editor = new EditorLoader(map, loadedMap);
        this.Push(editor);
    }

    public void ExportMapSet(RealmMapSet set)
    {
        if (set == null)
            return;

        var notification = new TaskNotificationData
        {
            Text = $"{set.Metadata.SortingTitle} - {set.Metadata.SortingArtist}",
            TextWorking = "Exporting..."
        };

        notifications.AddTask(notification);
        Task.Run(() => Maps.Export(set, notification));
    }

    public void DeleteMapSet(RealmMapSet set)
    {
        var items = Items.Where(i => i.Matches(set)).ToList();

        if (set == null || items.Count == 0)
            return;

        panels.Content ??= new ConfirmDeletionPanel(() =>
        {
            if (Equals(set, Maps.CurrentMapSet))
                changeItemSelection(1);

            Maps.DeleteMapSet(set);

            foreach (var item in items)
            {
                mapList.Remove(item);
                Items.Remove(item);
                item.Unbind();
            }

            if (Items.Count == 0)
                noMapsContainer.Show();
        }, itemName: "mapset");
    }

    private readonly List<IListItem> randomHistory = new();

    public void RandomMap()
    {
        if (Items.Count == 0)
            return;

        randomClick?.Play();

        var newItem = Items[RNG.Next(0, Items.Count)];
        var current = currentItem;

        newItem.Select();

        if (randomHistory.Count > 0)
        {
            var last = randomHistory.Last();
            if (!Equals(last, current))
                randomHistory.Add(current);
        }
        else
            randomHistory.Add(current);
    }

    public void RewindRandom()
    {
        if (randomHistory.Count <= 0)
            return;

        randomHistory.Last().Select();
        randomHistory.RemoveAt(randomHistory.Count - 1);
        rewindClick?.Play();
    }

    #endregion

    #region Sorting & Grouping

    public MapUtils.SortingMode SortMode => sortMode.Value;
    public bool SortInverse => sortInverse.Value;

    private Bindable<MapUtils.GroupingMode> groupMode;
    private Bindable<MapUtils.SortingMode> sortMode;
    private Bindable<bool> sortInverse;

    private void setSortingMode(ValueChangedEvent<MapUtils.SortingMode> e)
    {
        sortItems();
        mapList.Sort();
    }

    private void setGroupingMode(ValueChangedEvent<MapUtils.GroupingMode> e)
    {
        UpdateSearch();
    }

    #endregion

    #region Callbacks

    private bool firstMapUpdate = true;

    private void addMapSet(RealmMapSet set)
    {
        Scheduler.ScheduleIfNeeded(() =>
        {
            var items = createItems(set);

            mapList.StartBulkInsert();

            foreach (var item in items)
            {
                mapList.Insert(item);
                Items.Add(item);
            }

            sortItems();
            mapList.EndBulkInsert();

            noMapsContainer.Hide();
        });
    }

    private void mapBindableChanged(ValueChangedEvent<RealmMap> e)
    {
        var map = e.NewValue;

        if (map == null)
            return;

        var lastBackground = $"{e.OldValue.MapSet.ID}-{e.OldValue.Metadata.Background}";
        var newBackground = $"{e.NewValue.MapSet.ID}-{e.NewValue.Metadata.Background}";

        if (lastBackground != newBackground || firstMapUpdate)
        {
            var bg = backgroundVideo.Value ? new BlurableBackgroundWithStoryboard(map, BackgroundBlur) : new BlurableBackground(map, BackgroundBlur);
            backgrounds.PushBackground(bg);
        }

        menuScroll.Play();
        lightController.FadeColour(FluXisColors.GetKeyColor(map.KeyCount), 400);
        clock.AllowLimitedLoop = true;

        if (e.OldValue.FullAudioPath != e.NewValue.FullAudioPath)
            clock.Seek(Math.Max(0, map.Metadata.PreviewTime));

        var item = Items.FirstOrDefault(i => i.Matches(map));

        if (item is not null)
            ScheduleAfterChildren(() => mapList.ScrollToItem(item));

        firstMapUpdate = false;
    }

    private void updateBackgroundBlur(ValueChangedEvent<bool> e)
    {
        if (e.NewValue)
            backgrounds.SetBlur(BackgroundBlur);
        else
            backgrounds.SetBlur(0);
    }

    #endregion

    #region List Items

    protected List<IListItem> Items { get; } = new();
    private IListItem currentItem => Items.FirstOrDefault(i => i.State.Value == SelectedState.Selected);

    protected virtual bool ShouldAdd(RealmMapSet set) => true;

    private void loadMapSets()
    {
        var sets = Maps.MapSets;

        mapList.StartBulkInsert();

        foreach (RealmMapSet set in sets)
        {
            if (!ShouldAdd(set))
                continue;

            var items = createItems(set);

            foreach (var item in items)
            {
                Schedule(() => mapList.Insert(item));
                Items.Add(item);
            }
        }

        ScheduleAfterChildren(() =>
        {
            sortItems();
            mapList.EndBulkInsert();
        });

        if (!sets.Any())
            Schedule(noMapsContainer.Show);
    }

    private IEnumerable<IListItem> createItems(RealmMapSet set)
    {
        if (set is null || set.Maps.Count < 1)
            yield break;

        switch (groupMode.Value)
        {
            case MapUtils.GroupingMode.Default:
            {
                yield return new MapSetItem(set);

                break;
            }

            case MapUtils.GroupingMode.Nothing:
            {
                foreach (var map in set.Maps)
                    yield return new MapDifficultyItem(map);

                break;
            }
        }
    }

    private void sortItems() => Items.Sort();

    private void removeMapSet(RealmMapSet set) => Schedule(() =>
    {
        var results = Items.Where(i => i.Matches(set)).ToList();

        foreach (var item in results)
        {
            var idx = Items.IndexOf(item);

            if (Items.Count > results.Count)
            {
                while (Items[idx].Matches(set) && idx < 200)
                    idx++;

                Items[idx].Select();
            }

            mapList.Remove(item);
            Items.Remove(item);
            item.Unbind();
        }
    });

    private void replaceMapSet(RealmMapSet oldSet, RealmMapSet newSet)
    {
        Schedule(() =>
        {
            var results = Items.Where(i => i.Matches(oldSet)).ToList();

            foreach (var item in results)
            {
                mapList.Remove(item);
                Items.Remove(item);
                item.Unbind();
            }

            addMapSet(newSet);
        });
    }

    #endregion

    #region Input

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.PreviousGroup:
                changeItemSelection(-1);
                return true;

            case FluXisGlobalKeybind.Previous:
                changeSubItemSelection(-1);
                return true;

            case FluXisGlobalKeybind.NextGroup:
                changeItemSelection(1);
                return true;

            case FluXisGlobalKeybind.Next:
                changeSubItemSelection(1);
                return true;

            case FluXisGlobalKeybind.Select:
                footer.RightButton?.TriggerClick();
                return true;

            case FluXisGlobalKeybind.Back:
                if (modsOverlay.State.Value == Visibility.Visible)
                {
                    modsOverlay.Hide();
                    return true;
                }

                footer.LeftButton?.TriggerClick();
                return true;

            case FluXisGlobalKeybind.DecreaseRate:
                changeRate(-.05f);
                return true;

            case FluXisGlobalKeybind.IncreaseRate:
                changeRate(.05f);
                return true;
        }

        return false;

        void changeRate(float by) => modsOverlay.RateMod.RateBindable.Value += by;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.PageUp:
                changeLetter(-1);
                return true;

            case Key.PageDown:
                changeLetter(1);
                return true;

            default:
                if (e.ControlPressed || e.AltPressed || e.SuperPressed || e.ShiftPressed) break;

                var str = e.Key.ToString();

                if (str.Length != 1 || !char.IsLetter(str[0])) break;

                changeLetter(str[0]);
                return true;
        }

        return false;
    }

    private void changeItemSelection(int by = 0, bool last = false)
    {
        if (Items.Count == 0)
            return;

        var current = currentItem;

        int idx = Items.IndexOf(current);
        idx += by;

        if (idx < 0)
            idx = Items.Count - 1;
        else if (idx >= Items.Count)
            idx = 0;

        var item = Items[idx];
        item.Select(last);
    }

    private void changeSubItemSelection(int by = 0)
    {
        var item = currentItem;

        if (item is null)
        {
            if (Items.Count > 0)
                Items.First().Select();

            return;
        }

        if (!item.ChangeChild(by))
            return;

        changeItemSelection(by, by < 0);
    }

    #endregion

    #region Screen Transitions

    public override void OnEntering(ScreenTransitionEvent e)
    {
        playEnterAnimation();

        songSelectBlur.ValueChanged += updateBackgroundBlur;

        if (Maps.CurrentMap != null)
            clock.RestartPoint = Maps.CurrentMap.Metadata.PreviewTime;
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        clock.Looping = false;
        songSelectBlur.ValueChanged -= updateBackgroundBlur;

        if (e.Next is EditorLoader)
            this.Delay(EditorLoader.DURATION).FadeOut();
        else
            playExitAnimation();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        if (e.Last is EditorLoader)
            this.FadeIn();
        else
            playEnterAnimation();

        songSelectBlur.ValueChanged += updateBackgroundBlur;

        if (Maps.CurrentMap != null)
        {
            lightController.FadeColour(FluXisColors.GetKeyColor(Maps.CurrentMap.KeyCount), 400);

            clock.RestartPoint = Maps.CurrentMap.Metadata.PreviewTime;

            if (!clock.IsRunning)
                clock.Start();
        }

        selectMapInfo.ScoreList?.Refresh();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        clock.Looping = false;
        clock.RateTo(1f, 400, Easing.Out);

        playExitAnimation();
        return base.OnExiting(e);
    }

    private void playEnterAnimation()
    {
        clock.AllowLimitedLoop = true;
        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.FadeIn(FADE_DURATION);

            mapList.MoveToX(-100)
                   .MoveToX(0, MOVE_DURATION, Easing.OutQuint);

            selectMapInfo.MoveToX(100)
                         .MoveToX(0, MOVE_DURATION, Easing.OutQuint);

            searchBar.Show();
            filterControl.Show();
            footer.Show();
        }
    }

    private void playExitAnimation()
    {
        clock.AllowLimitedLoop = false;
        this.FadeOut(FADE_DURATION);

        mapList.MoveToX(-100, MOVE_DURATION, Easing.OutQuint);
        selectMapInfo.MoveToX(100, MOVE_DURATION, Easing.OutQuint);
        searchBar.Hide();
        filterControl.Hide();
        footer.Hide();
        noMapsContainer.Hide();
    }

    #endregion

    #region Letters

    private static char[] letters => "#ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    private static char getLetter(char letter)
    {
        letter = char.ToUpper(letter);
        if (!letters.Contains(letter)) letter = '#';
        return letter;
    }

    private void changeLetter(int by)
    {
        if (Items.Count <= 1)
            return;

        var title = Maps.CurrentMapSet.Metadata.SortingTitle;
        var current = title.Length < 1 ? '#' : getLetter(Maps.CurrentMapSet.Metadata.SortingTitle[0]);

        var index = Array.IndexOf(letters, current);
        index += by;

        if (index < 0) index = letters.Length - 1;
        if (index >= letters.Length) index = 0;

        while (Items.All(m => getLetter(m.Metadata.SortingTitle.FirstOrDefault(' ')) != letters[index]))
        {
            index += by;

            if (index < 0) index = letters.Length - 1;
            if (index >= letters.Length) index = 0;

            if (index == Array.IndexOf(letters, current))
                break;
        }

        var newLetter = letters[index];
        changeLetter(newLetter);
    }

    private void changeLetter(char letter)
    {
        if (Items.Count <= 1)
            return;

        var item = currentItem;
        var title = item.Metadata.SortingTitle;
        var current = title.Length < 1 ? '#' : getLetter(title[0]);

        if (current == letter)
        {
            cycleLetter(letter);
            return;
        }

        var first = Items.FirstOrDefault(i => getLetter(i.Metadata.SortingTitle.FirstOrDefault(' ')) == letter);
        first?.Select();

        letterContainer.SetLetter(letter, first is not null);
    }

    private void cycleLetter(char letter)
    {
        if (Items.Count <= 1) // no need to change letter if there's only one map
            return;

        var item = currentItem;

        var startingWith = Items.Where(i => getLetter(i.Metadata.SortingTitle.FirstOrDefault(' ')) == letter).ToList();
        var idx = startingWith.IndexOf(item);

        if (idx == -1)
            return;

        idx++;
        if (idx >= startingWith.Count) idx = 0;

        var next = startingWith[idx];
        next.Select();
    }

    #endregion

    #region Search

    protected SearchFilters Filters { get; private set; }
    private IdleTracker searchTracker;

    protected void UpdateSearch()
    {
        var oldItems = Items.ToList();

        foreach (var item in oldItems)
        {
            mapList.Remove(item);
            Items.Remove(item);
            item.Unbind();
        }

        mapList.StartBulkInsert();

        foreach (var set in Maps.MapSets)
        {
            if (!ShouldAdd(set))
                continue;

            var items = createItems(set);

            foreach (var item in items)
            {
                if (item.MatchesFilter(Filters))
                {
                    Items.Add(item);
                    mapList.Insert(item);
                }
            }
        }

        ScheduleAfterChildren(() =>
        {
            sortItems();
            mapList.EndBulkInsert();

            if (Items.Count == 1)
                Items.First().Select();

            if (!Items.Any())
                noMapsContainer.Show();
            else
                noMapsContainer.Hide();
        });
    }

    #endregion
}
