using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Audio.Transforms;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.IO;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Online.Activity;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Replays;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Gameplay.Replays;
using fluXis.Game.Screens.Select.Footer;
using fluXis.Game.Screens.Select.Info;
using fluXis.Game.Screens.Select.List;
using fluXis.Game.Screens.Select.Mods;
using fluXis.Game.Screens.Select.Search;
using fluXis.Game.Screens.Select.UI;
using fluXis.Game.Utils;
using fluXis.Game.Utils.Extensions;
using fluXis.Shared.Replays;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Utils;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Select;

public partial class SelectScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.1f;
    public override float BackgroundBlur => songSelectBlur.Value ? 0.25f : 0;
    public override bool ApplyValuesAfterLoad => true;

    public override UserActivity InitialActivity => new UserActivity.SongSelect();

    [Resolved]
    protected MapStore MapStore { get; private set; }

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

    private DependencyContainer dependencies;

    protected List<RealmMapSet> Maps { get; } = new();
    protected MapList MapList { get; private set; }
    protected SearchFilters Filters { get; private set; }

    private SelectMapInfo selectMapInfo;
    private SearchBar searchBar;
    private SelectFooter footer;
    private ModSelector modSelector;

    private Sample menuScroll;
    private Sample randomClick;
    private Sample rewindClick;

    private SelectNoMaps noMapsContainer;
    private LoadingIcon loadingIcon;
    private SelectLetter letterContainer;

    private readonly List<RealmMapSet> randomHistory = new();

    private readonly Dictionary<RealmMapSet, MapListEntry> lookup = new();

    private Bindable<bool> songSelectBlur;

    private InputManager inputManager;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config)
    {
        menuScroll = samples.Get("UI/scroll.mp3");
        randomClick = samples.Get("UI/Select/Random.wav");
        rewindClick = samples.Get("UI/Select/Rewind.wav");

        songSelectBlur = config.GetBindable<bool>(FluXisSetting.SongSelectBlur);

        dependencies.CacheAs(this);
        dependencies.CacheAs(Filters = new SearchFilters());
        dependencies.CacheAs(modSelector = new ModSelector());

        Filters.OnChange += UpdateSearch;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = -10 },
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = ColourInfo.GradientVertical(Colour4.Black.Opacity(.8f), Colour4.Black.Opacity(0))
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Bottom = 50 },
                Children = new Drawable[]
                {
                    new FluXisContextMenuContainer
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.Both,
                        Width = .5f,
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding(10)
                            {
                                Top = 80,
                                Left = 20,
                                Bottom = 40
                            },
                            Child = MapList = new MapList { Alpha = 0 }
                        }
                    },
                    searchBar = new SearchBar(),
                    selectMapInfo = new SelectMapInfo
                    {
                        HoverAction = MapList.ScrollToSelected
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
            footer = new SelectFooter
            {
                BackAction = this.Exit,
                ModsAction = modSelector.IsOpen.Toggle,
                RewindAction = RewindRandom,
                RandomAction = RandomMap,
                PlayAction = Accept,
                DeleteAction = DeleteMapSet,
                EditAction = EditMap,
                ScoresWiped = () => selectMapInfo.ScoreList.Refresh()
            },
            modSelector
        };
    }

    protected override void LoadComplete()
    {
        inputManager = GetContainingInputManager();

        if (MapStore.CurrentMap.Hash == "dummy" && MapStore.MapSets.Any())
            MapStore.Select(MapStore.GetRandom()?.LowestDifficulty, true);

        Task.Run(() =>
        {
            Logger.Log("Loading sets...", LoggingTarget.Runtime, LogLevel.Debug);

            loadMapSets();

            Schedule(() =>
            {
                MapStore.MapSetAdded += addMapSet;
                MapStore.MapSetUpdated += replaceMapSet;
                MapStore.MapSetBindable.BindValueChanged(mapSetBindableChanged, true);
                MapStore.MapBindable.BindValueChanged(mapBindableChanged, true);

                MapList.FadeIn(500);
                loadingIcon.FadeOut(500);
                OnMapsLoaded();
            });
        });
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        MapStore.MapSetAdded -= addMapSet;
        MapStore.MapSetUpdated -= replaceMapSet;
        songSelectBlur.ValueChanged -= updateBackgroundBlur;

        MapStore.MapSetBindable.ValueChanged -= mapSetBindableChanged;
        MapStore.MapBindable.ValueChanged -= mapBindableChanged;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    private void loadMapSets()
    {
        var sets = MapStore.MapSets;

        MapList.StartBulkInsert();

        foreach (RealmMapSet set in sets)
        {
            var entry = new MapListEntry(set)
            {
                SelectAction = Accept,
                EditAction = EditMap,
                DeleteAction = DeleteMapSet,
                ExportAction = ExportMapSet
            };

            LoadComponent(entry);
            Schedule(() => MapList.Insert(entry));
            lookup[set] = entry;
            Maps.Add(set);
        }

        ScheduleAfterChildren(() => MapList.EndBulkInsert());

        if (!sets.Any())
            Schedule(noMapsContainer.Show);
    }

    private void addMapSet(RealmMapSet set)
    {
        Scheduler.ScheduleIfNeeded(() =>
        {
            var entry = new MapListEntry(set)
            {
                SelectAction = Accept,
                EditAction = EditMap,
                DeleteAction = DeleteMapSet,
                ExportAction = ExportMapSet
            };

            LoadComponentAsync(entry, _ =>
            {
                MapList.Insert(entry);
                Maps.Add(set);
                lookup[set] = entry;
                noMapsContainer.Hide();
            });
        });
    }

    private void replaceMapSet(RealmMapSet oldSet, RealmMapSet newSet)
    {
        Schedule(() =>
        {
            if (lookup.ContainsKey(oldSet))
            {
                MapList.Remove(lookup[oldSet], true);
                Maps.Remove(oldSet);
                lookup.Remove(oldSet);
            }

            addMapSet(newSet);
            // Schedule(() => { Schedule(() => mapStore.Select(newSet.LowestDifficulty)); }); // <- this looks stupid and probably is, but it works
        });
    }

    private void mapSetBindableChanged(ValueChangedEvent<RealmMapSet> e) => selectMapSet(e.NewValue);
    private void mapBindableChanged(ValueChangedEvent<RealmMap> e) => selectMap(e.NewValue);

    private void selectMapSet(RealmMapSet set)
    {
        if (set == null || !lookup.ContainsKey(set))
            return;

        var previous = MapStore.CurrentMapSet;

        if (!Equals(previous, set) || !clock.IsRunning)
            clock.Seek(set.Metadata.PreviewTime);
    }

    private void selectMap(RealmMap map)
    {
        if (map == null)
            return;

        menuScroll.Play();
        backgrounds.AddBackgroundFromMap(map);
        lightController.FadeColour(FluXisColors.GetKeyColor(map.KeyCount), 400);

        if (lookup.TryGetValue(map.MapSet, out var entry))
            ScheduleAfterChildren(() => MapList.ScrollTo(entry));
    }

    protected virtual void Accept()
    {
        if (MapStore.CurrentMap == null)
            return;

        UISamples.Select();
        backgrounds.AddBackgroundFromMap(MapStore.CurrentMap);
        backgrounds.SwipeAnimation();

        var mods = modSelector.SelectedMods.ToList();

        if (inputManager.CurrentState.Keyboard.ControlPressed && !mods.Any(m => m is AutoPlayMod))
            mods.Add(new AutoPlayMod());

        if (mods.Any(m => m is AutoPlayMod))
        {
            viewReplay(MapStore.CurrentMap, mods, () =>
            {
                var map = MapStore.CurrentMap.GetMapInfo();
                var autogen = new AutoGenerator(map, MapStore.CurrentMap.KeyCount);
                return autogen.Generate();
            });
        }
        else
            this.Push(new GameplayLoader(MapStore.CurrentMap, mods, () => new GameplayScreen(MapStore.CurrentMap, mods)));
    }

    public void ViewReplay(RealmMap map, RealmScore score)
    {
        var scoreMods = score.Mods.Split(' ').ToList();
        scoreMods.RemoveAll(string.IsNullOrEmpty);

        var mods = scoreMods.Select(ModUtils.GetFromAcronym).ToList();
        mods.RemoveAll(m => m == null);

        if (mods.Count != scoreMods.Count)
        {
            Logger.Log($"Some mods were not found ({mods.Count}:{scoreMods.Count})", LoggingTarget.Runtime, LogLevel.Error);
            return;
        }

        viewReplay(map, mods, () =>
        {
            var replay = replays.Get(score.ID);

            if (replay == null)
            {
                Logger.Log($"Replay for score {score.ID} not found", LoggingTarget.Runtime, LogLevel.Error);
                return null;
            }

            return replay;
        });
    }

    private void viewReplay(RealmMap map, List<IMod> mods, Func<Replay> replayFunc)
    {
        this.Push(new GameplayLoader(map, mods, () =>
        {
            var replay = replayFunc();
            return replay == null ? null : new ReplayGameplayScreen(map, mods, replay);
        }));
    }

    private void changeSelection(int by = 0, bool last = false)
    {
        if (Maps.Count == 0)
            return;

        int current = Maps.IndexOf(MapStore.CurrentMapSet);
        current += by;

        if (current < 0)
            current = Maps.Count - 1;
        else if (current >= Maps.Count)
            current = 0;

        var set = Maps[current];
        var map = last ? set.HighestDifficulty : set.LowestDifficulty;
        MapStore.Select(map, true);
    }

    private void changeMapSelection(int by = 0)
    {
        if (!Maps.Contains(MapStore.CurrentMapSet)) return;

        var listEntry = lookup[MapStore.CurrentMapSet];

        int current = listEntry.Maps.IndexOf(MapStore.CurrentMap);
        current += by;

        if (current < 0)
        {
            changeSelection(-1, true);
            return;
        }

        if (current >= listEntry.Maps.Count)
        {
            changeSelection(1);
            return;
        }

        MapStore.Select(listEntry.Maps[current], true);
    }

    public void DeleteMapSet(RealmMapSet set)
    {
        if (set == null)
            return;

        panels.Content ??= new ConfirmDeletionPanel(() =>
        {
            MapStore.DeleteMapSet(set);
            MapList.Remove(lookup[set], false);
            Maps.Remove(set);
            lookup.Remove(set);

            if (Equals(set, MapStore.CurrentMapSet))
                changeSelection(1);

            if (Maps.Count == 0)
                noMapsContainer.Show();
        }, itemName: "MapSet");
    }

    public void ExportMapSet(RealmMapSet set)
    {
        if (set == null)
            return;

        var notification = new TaskNotificationData
        {
            Text = $"{set.Metadata.Title} - {set.Metadata.Artist}",
            TextWorking = "Exporting..."
        };

        notifications.AddTask(notification);
        Task.Run(() => MapStore.Export(set, notification));
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(200);
        clock.Looping = false;
        songSelectBlur.ValueChanged -= updateBackgroundBlur;

        MapList.MoveToX(-200, 500, Easing.OutQuint);
        selectMapInfo.MoveToX(200, 500, Easing.OutQuint);
        searchBar.Hide();
        footer.Hide();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        this.FadeIn(200);
        lightController.FadeColour(FluXisColors.GetKeyColor(MapStore.CurrentMap.KeyCount), 400);
        songSelectBlur.ValueChanged += updateBackgroundBlur;

        MapList.MoveToX(0, 500, Easing.OutQuint);
        selectMapInfo.MoveToX(0, 500, Easing.OutQuint);
        searchBar.Show();
        footer.Show();

        if (MapStore.CurrentMap != null)
        {
            clock.RestartPoint = MapStore.CurrentMap.Metadata.PreviewTime;

            if (!clock.IsRunning)
                clock.Start();
        }

        selectMapInfo.ScoreList.Refresh();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(200);
        songSelectBlur.ValueChanged += updateBackgroundBlur;

        MapList.MoveToX(-200)
               .MoveToX(0, 500, Easing.OutQuint);

        selectMapInfo.MoveToX(200)
                     .MoveToX(0, 500, Easing.OutQuint);

        searchBar.Show();
        footer.Show();

        if (MapStore.CurrentMap != null)
            clock.RestartPoint = MapStore.CurrentMap.Metadata.PreviewTime;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(200);
        clock.Looping = false;

        MapList.MoveToX(-200, 500, Easing.OutQuint);
        selectMapInfo.MoveToX(200, 500, Easing.OutQuint);
        searchBar.Hide();
        footer.Hide();

        clock.RateTo(1f);

        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.PreviousGroup:
                changeSelection(-1);
                return true;

            case FluXisGlobalKeybind.Previous:
                changeMapSelection(-1);
                return true;

            case FluXisGlobalKeybind.NextGroup:
                changeSelection(1);
                return true;

            case FluXisGlobalKeybind.Next:
                changeMapSelection(1);
                return true;

            case FluXisGlobalKeybind.Select:
                Accept();
                return true;

            case FluXisGlobalKeybind.Back:
                if (modSelector.IsOpen.Value)
                {
                    modSelector.IsOpen.Value = false;
                    return true;
                }

                this.Exit();
                return true;

            case FluXisGlobalKeybind.DecreaseRate:
                changeRate(-.05f);
                return true;

            case FluXisGlobalKeybind.IncreaseRate:
                changeRate(.05f);
                return true;
        }

        return false;
    }

    private void changeRate(float by) => modSelector.RateMod.RateBindable.Value += by;

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case >= Key.F1 and <= Key.F12:
                var index = (int)e.Key - (int)Key.F1;

                if (index < footer.ButtonContainer.Count)
                    footer.ButtonContainer[index].TriggerClick();

                return true;

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

    private static char[] letters => "#ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    private static char getLetter(char letter)
    {
        letter = char.ToUpper(letter);
        if (!letters.Contains(letter)) letter = '#';
        return letter;
    }

    private void changeLetter(int by)
    {
        if (Maps.Count <= 1) // no need to change letter if there's only one map
            return;

        var current = getLetter(MapStore.CurrentMapSet.Metadata.Title[0]);

        var index = Array.IndexOf(letters, current);
        index += by;

        if (index < 0) index = letters.Length - 1;
        if (index >= letters.Length) index = 0;

        while (Maps.All(m => getLetter(m.Metadata.Title.FirstOrDefault(' ')) != letters[index]))
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
        if (Maps.Count <= 1) // no need to change letter if there's only one map
            return;

        var current = getLetter(MapStore.CurrentMapSet.Metadata.Title[0]);

        if (current == letter)
        {
            cycleLetter(letter);
            return;
        }

        Logger.Log($"Changing letter to {letter}");

        var first = Maps.FirstOrDefault(m => getLetter(m.Metadata.Title.FirstOrDefault(' ')) == letter);

        if (first != null)
            MapStore.CurrentMapSet = first;

        letterContainer.SetLetter(letter);
    }

    private void cycleLetter(char letter)
    {
        if (Maps.Count <= 1) // no need to change letter if there's only one map
            return;

        var startingWith = Maps.Where(m => getLetter(m.Metadata.Title.FirstOrDefault(' ')) == letter).ToList();
        var idx = startingWith.IndexOf(MapStore.CurrentMapSet);

        if (idx == -1) return;

        idx++;
        if (idx >= startingWith.Count) idx = 0;

        MapStore.CurrentMapSet = startingWith[idx];
    }

    protected virtual void OnMapsLoaded() { }

    protected virtual void UpdateSearch()
    {
        Maps.Clear();

        foreach (var child in MapList.Content.Children)
        {
            bool matches = child.MapSet.Maps.Aggregate(false, (current, map) => current | Filters.Matches(map));

            if (matches)
            {
                Maps.Add(child.MapSet);
                child.Show();
            }
            else
                child.Hide();
        }

        if (!Maps.Any())
            noMapsContainer.Show();
        else
            noMapsContainer.Hide();
    }

    public void RandomMap()
    {
        if (Maps.Count == 0)
            return;

        var newMap = Maps[RNG.Next(0, Maps.Count)];
        var currentMap = MapStore.CurrentMapSet;

        MapStore.CurrentMapSet = newMap;
        randomClick?.Play();

        if (randomHistory.Count > 0)
        {
            var last = randomHistory.Last();
            if (!Equals(last, currentMap))
                randomHistory.Add(currentMap);
        }
        else
            randomHistory.Add(currentMap);
    }

    public void RewindRandom()
    {
        if (randomHistory.Count <= 0)
            return;

        MapStore.CurrentMapSet = randomHistory.Last();
        randomHistory.RemoveAt(randomHistory.Count - 1);
        rewindClick?.Play();
    }

    private void updateBackgroundBlur(ValueChangedEvent<bool> e)
    {
        if (e.NewValue)
            backgrounds.SetBlur(BackgroundBlur, 500);
        else
            backgrounds.SetBlur(0, 500);
    }

    public void EditMap(RealmMap map)
    {
        if (map == null) return;

        MapStore.Select(map, true);
        if (MapStore.CurrentMap == null) return;

        var loadedMap = map.GetMapInfo();
        if (loadedMap == null) return;

        var editor = new EditorLoader(map, loadedMap);
        this.Push(editor);
    }
}
