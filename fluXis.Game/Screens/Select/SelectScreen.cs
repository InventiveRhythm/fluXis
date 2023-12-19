using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Audio.Transforms;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Online.Activity;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Types.Loading;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Select.Footer;
using fluXis.Game.Screens.Select.Info;
using fluXis.Game.Screens.Select.List;
using fluXis.Game.Screens.Select.Mods;
using fluXis.Game.Screens.Select.Search;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
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
    private MapStore mapStore { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private LightController lightController { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    private readonly List<RealmMapSet> maps = new();
    public SearchFilters Filters = new();

    private MapList mapList;
    private SelectMapInfo selectMapInfo;
    private SearchBar searchBar;
    private SelectFooter footer;
    public ModSelector ModSelector;

    private Sample menuAccept;
    private Sample menuScroll;
    private Sample randomClick;
    private Sample rewindClick;

    private Container noMapsContainer;
    private LoadingIcon loadingIcon;
    private Container letterContainer;
    private FluXisSpriteText currentLetter;

    private readonly List<RealmMapSet> randomHistory = new();

    private readonly Dictionary<RealmMapSet, MapListEntry> lookup = new();

    private Bindable<bool> songSelectBlur;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config)
    {
        menuAccept = samples.Get("UI/accept.mp3");
        menuScroll = samples.Get("UI/scroll.mp3");
        randomClick = samples.Get("UI/Select/Random.wav");
        rewindClick = samples.Get("UI/Select/Rewind.wav");

        Filters.OnChange += updateSearch;
        songSelectBlur = config.GetBindable<bool>(FluXisSetting.SongSelectBlur);

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
                            Padding = new MarginPadding(10) { Top = 80, Left = 20, Bottom = 40 },
                            Child = mapList = new MapList { Alpha = 0 }
                        }
                    },
                    searchBar = new SearchBar { Filters = Filters },
                    selectMapInfo = new SelectMapInfo
                    {
                        Screen = this,
                        HoverAction = mapList.ScrollToSelected
                    },
                    new Container
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.Both,
                        Width = .5f,
                        Children = new Drawable[]
                        {
                            noMapsContainer = new Container
                            {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                CornerRadius = 20,
                                Masking = true,
                                Alpha = 0,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Colour4.Black,
                                        Alpha = 0.5f
                                    },
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Padding = new MarginPadding(20),
                                        Children = new Drawable[]
                                        {
                                            new SpriteIcon
                                            {
                                                Icon = FontAwesome.Solid.ExclamationTriangle,
                                                Size = new Vector2(30),
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre
                                            },
                                            new FluXisSpriteText
                                            {
                                                Text = "No maps found!",
                                                FontSize = 32,
                                                Shadow = true,
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre
                                            },
                                            new FluXisSpriteText
                                            {
                                                Text = "Try changing your search filters.",
                                                FontSize = 26,
                                                Colour = FluXisColors.Text2,
                                                Shadow = true,
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre
                                            }
                                        }
                                    }
                                }
                            },
                            letterContainer = new Container
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(100),
                                Alpha = 0,
                                CornerRadius = 20,
                                Masking = true,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        Alpha = 0.5f,
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Colour4.Black
                                    },
                                    currentLetter = new FluXisSpriteText
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        FontSize = 64,
                                        Text = "A"
                                    }
                                }
                            },
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
            footer = new SelectFooter { Screen = this },
            ModSelector = new ModSelector()
        };
    }

    protected override void LoadComplete()
    {
        if (mapStore.CurrentMap.Hash == "dummy" && mapStore.MapSetsSorted.Any())
            mapStore.Select(mapStore.MapSetsSorted.First().LowestDifficulty, true);

        Task.Run(() =>
        {
            Logger.Log("Loading sets...", LoggingTarget.Runtime, LogLevel.Debug);

            loadMapSets();

            Schedule(() =>
            {
                mapStore.MapSetAdded += addMapSet;
                mapStore.MapSetUpdated += replaceMapSet;
                mapStore.MapSetBindable.BindValueChanged(mapSetBindableChanged, true);
                mapStore.MapBindable.BindValueChanged(mapBindableChanged, true);

                mapList.FadeIn(500);
                loadingIcon.FadeOut(500);
            });
        });
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        mapStore.MapSetAdded -= addMapSet;
        mapStore.MapSetUpdated -= replaceMapSet;
        songSelectBlur.ValueChanged -= updateBackgroundBlur;

        mapStore.MapSetBindable.ValueChanged -= mapSetBindableChanged;
        mapStore.MapBindable.ValueChanged -= mapBindableChanged;
    }

    private void loadMapSets()
    {
        var sets = mapStore.MapSetsSorted;

        foreach (RealmMapSet set in sets)
        {
            var entry = new MapListEntry(set)
            {
                SelectAction = Accept,
                EditAction = EditMapSet,
                DeleteAction = DeleteMapSet,
                ExportAction = ExportMapSet
            };

            LoadComponent(entry);
            Schedule(() => mapList.AddMap(entry));
            lookup[set] = entry;
            maps.Add(set);
        }

        if (!sets.Any()) Schedule(() => noMapsContainer.FadeIn(500));
    }

    private void addMapSet(RealmMapSet set)
    {
        Schedule(() =>
        {
            int index = mapStore.MapSetsSorted.IndexOf(set);
            if (index == -1) return;

            var entry = new MapListEntry(set)
            {
                SelectAction = Accept,
                EditAction = EditMapSet,
                DeleteAction = DeleteMapSet,
                ExportAction = ExportMapSet
            };

            mapList.Insert(index, entry);
            lookup[set] = entry;
            // mapStore.Select(set.LowestDifficulty, true);
            noMapsContainer.FadeOut(200);
        });
    }

    private void replaceMapSet(RealmMapSet oldSet, RealmMapSet newSet)
    {
        Schedule(() =>
        {
            if (lookup.ContainsKey(oldSet))
            {
                mapList.Remove(lookup[oldSet], true);
                maps.Remove(oldSet);
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

        var previous = mapStore.CurrentMapSet;

        if (!Equals(previous, set) || !clock.IsRunning)
            clock.Seek(set.Metadata.PreviewTime);
    }

    private void selectMap(RealmMap map)
    {
        if (map == null)
            return;

        menuScroll.Play();
        backgrounds.AddBackgroundFromMap(map);
        selectMapInfo.ChangeMap(map);
        lightController.FadeColour(FluXisColors.GetKeyColor(map.KeyCount), 400);

        if (lookup.TryGetValue(map.MapSet, out var entry))
            ScheduleAfterChildren(() => mapList.ScrollTo(entry));
    }

    public void Accept()
    {
        if (mapStore.CurrentMap == null)
            return;

        menuAccept.Play();
        backgrounds.AddBackgroundFromMap(mapStore.CurrentMap);
        backgrounds.SwipeAnimation();

        this.Push(new GameplayLoader(mapStore.CurrentMap, () => new GameplayScreen(mapStore.CurrentMap, ModSelector.SelectedMods)));
    }

    private void changeSelection(int by = 0, bool last = false)
    {
        if (maps.Count == 0)
            return;

        int current = maps.IndexOf(mapStore.CurrentMapSet);
        current += by;

        if (current < 0)
            current = maps.Count - 1;
        else if (current >= maps.Count)
            current = 0;

        var set = maps[current];
        var map = last ? set.HighestDifficulty : set.LowestDifficulty;
        mapStore.Select(map, true);
    }

    private void changeMapSelection(int by = 0)
    {
        if (!maps.Contains(mapStore.CurrentMapSet)) return;

        var listEntry = lookup[mapStore.CurrentMapSet];

        int current = listEntry.Maps.IndexOf(mapStore.CurrentMap);
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

        mapStore.Select(listEntry.Maps[current], true);
    }

    public void OpenDeleteConfirm(RealmMapSet set)
    {
        Game.Overlay ??= new ConfirmDeletionPanel(() => DeleteMapSet(set), itemName: "MapSet") { ButtonWidth = 200 };
    }

    public void DeleteMapSet(RealmMapSet set)
    {
        if (set == null)
            return;

        mapStore.DeleteMapSet(set);
        mapList.Remove(lookup[set], false);
        maps.Remove(set);
        lookup.Remove(set);

        if (Equals(set, mapStore.CurrentMapSet))
            changeSelection(1);

        if (maps.Count == 0)
            noMapsContainer.FadeIn(500);
    }

    public void ExportMapSet(RealmMapSet set)
    {
        if (set == null)
            return;

        var notification = new LoadingNotificationData
        {
            TextLoading = $"Exporting mapset {set.Metadata.Artist} - {set.Metadata.Title}...",
            TextSuccess = $"Exported mapset {set.Metadata.Artist} - {set.Metadata.Title}!",
            TextFailure = $"Failed to export mapset {set.Metadata.Artist} - {set.Metadata.Title}!"
        };

        notifications.Add(notification);
        Task.Run(() => mapStore.Export(set, notification));
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(200);
        clock.Looping = false;
        songSelectBlur.ValueChanged -= updateBackgroundBlur;

        mapList.MoveToX(-200, 500, Easing.OutQuint);
        selectMapInfo.MoveToX(200, 500, Easing.OutQuint);
        searchBar.Hide();
        footer.Hide();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        this.FadeIn(200);
        lightController.FadeColour(FluXisColors.GetKeyColor(mapStore.CurrentMap.KeyCount), 400);
        songSelectBlur.ValueChanged += updateBackgroundBlur;

        mapList.MoveToX(0, 500, Easing.OutQuint);
        selectMapInfo.MoveToX(0, 500, Easing.OutQuint);
        searchBar.Show();
        footer.Show();

        if (mapStore.CurrentMap != null)
        {
            clock.RestartPoint = mapStore.CurrentMap.Metadata.PreviewTime;

            if (!clock.IsRunning)
                clock.Start();
        }

        selectMapInfo.ScoreList.Refresh();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(200);
        songSelectBlur.ValueChanged += updateBackgroundBlur;

        mapList.MoveToX(-200)
               .MoveToX(0, 500, Easing.OutQuint);

        selectMapInfo.MoveToX(200)
                     .MoveToX(0, 500, Easing.OutQuint);

        searchBar.Show();
        footer.Show();

        if (mapStore.CurrentMap != null)
            clock.RestartPoint = mapStore.CurrentMap.Metadata.PreviewTime;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(200);
        clock.Looping = false;

        mapList.MoveToX(-200, 500, Easing.OutQuint);
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
                if (ModSelector.IsOpen.Value)
                {
                    ModSelector.IsOpen.Value = false;
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

    private void changeRate(float by) => ModSelector.RateMod.RateBindable.Value += by;

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
        if (maps.Count <= 1) // no need to change letter if there's only one map
            return;

        var current = getLetter(mapStore.CurrentMapSet.Metadata.Title[0]);

        var index = Array.IndexOf(letters, current);
        index += by;

        if (index < 0) index = letters.Length - 1;
        if (index >= letters.Length) index = 0;

        while (maps.All(m => getLetter(m.Metadata.Title.FirstOrDefault(' ')) != letters[index]))
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
        if (maps.Count <= 1) // no need to change letter if there's only one map
            return;

        var current = getLetter(mapStore.CurrentMapSet.Metadata.Title[0]);

        if (current == letter)
        {
            cycleLetter(letter);
            return;
        }

        Logger.Log($"Changing letter to {letter}");

        var first = maps.FirstOrDefault(m => getLetter(m.Metadata.Title.FirstOrDefault(' ')) == letter);
        if (first != null) mapStore.CurrentMapSet = first;

        currentLetter.Text = letter.ToString();
        letterContainer.FadeIn(200).Delay(1000).FadeOut(300);

        if (first == null)
        {
            currentLetter.FadeColour(Colour4.FromHex("#FF5555")).FadeColour(Colour4.White, 1000);
        }
    }

    private void cycleLetter(char letter)
    {
        if (maps.Count <= 1) // no need to change letter if there's only one map
            return;

        var startingWith = maps.Where(m => getLetter(m.Metadata.Title.FirstOrDefault(' ')) == letter).ToList();
        var idx = startingWith.IndexOf(mapStore.CurrentMapSet);

        if (idx == -1) return;

        idx++;
        if (idx >= startingWith.Count) idx = 0;

        mapStore.CurrentMapSet = startingWith[idx];
    }

    private void updateSearch()
    {
        maps.Clear();

        foreach (var child in mapList.Content.Children)
        {
            bool matches = child.MapSet.Maps.Aggregate(false, (current, map) => current | Filters.Matches(map));

            if (matches)
            {
                maps.Add(child.MapSet);
                child.Show();
            }
            else
                child.Hide();
        }

        if (!maps.Any())
            noMapsContainer.FadeIn(200);
        else
            noMapsContainer.FadeOut(200);
    }

    public void RandomMap()
    {
        if (maps.Count == 0)
            return;

        var newMap = maps[RNG.Next(0, maps.Count)];
        var currentMap = mapStore.CurrentMapSet;

        mapStore.CurrentMapSet = newMap;
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

        mapStore.CurrentMapSet = randomHistory.Last();
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

    public void EditMapSet(RealmMap map)
    {
        if (map == null) return;

        mapStore.Select(map, true);
        if (mapStore.CurrentMap == null) return;

        var loadedMap = map.GetMapInfo();
        if (loadedMap == null) return;

        var editor = new EditorLoader(map, loadedMap);
        this.Push(editor);
    }
}
