using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Activity;
using fluXis.Game.Audio;
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

public partial class SelectScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float Zoom => 1.1f;
    public override float BackgroundBlur => songSelectBlur.Value ? 0.25f : 0;
    public override bool ApplyValuesAfterLoad => true;

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private LightController lightController { get; set; }

    [Resolved]
    private AudioClock clock { get; set; }

    [Resolved]
    private ActivityManager activity { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    public Bindable<RealmMapSet> MapSet = new();
    public Bindable<RealmMap> MapInfo = new();
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
                            Padding = new MarginPadding(10) { Top = 80, Left = 20 },
                            Child = mapList = new MapList { Alpha = 0 }
                        }
                    },
                    searchBar = new SearchBar { Filters = Filters },
                    selectMapInfo = new SelectMapInfo { Screen = this },
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

        MapSet.ValueChanged += e => selectMapSet(e.NewValue);
        MapInfo.ValueChanged += e => selectMap(e.NewValue);
    }

    private void loadMapSets()
    {
        var sets = mapStore.MapSetsSorted;

        foreach (RealmMapSet set in sets)
        {
            MapListEntry entry = new(this, set);
            LoadComponent(entry);
            Schedule(() => mapList.AddMap(entry));
            maps.Add(set);
            lookup[set] = entry;
        }

        if (!sets.Any()) Schedule(() => noMapsContainer.FadeIn(500));
    }

    protected override void LoadComplete()
    {
        mapStore.MapSetAdded += addMapSet;
        mapStore.MapSetUpdated += replaceMapSet;

        Task.Run(() =>
        {
            Logger.Log("Loading sets...", LoggingTarget.Runtime, LogLevel.Debug);

            loadMapSets();

            Schedule(() =>
            {
                if (maps.Count > 0)
                    MapSet.Value = mapStore.CurrentMapSet;

                mapList.FadeIn(500);
                loadingIcon.FadeOut(500);
            });
        });
    }

    private void addMapSet(RealmMapSet set)
    {
        Schedule(() =>
        {
            int index = mapStore.MapSetsSorted.IndexOf(set);
            if (index == -1) return;

            MapListEntry entry = new(this, set);
            maps.Insert(index, set);
            mapList.Insert(index, entry);
            lookup[set] = entry;

            MapSet.Value = set;

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
            Schedule(() => { Schedule(() => selectMapSet(newSet)); }); // <- this looks stupid and probably is, but it works
        });
    }

    private void selectMapSet(RealmMapSet set)
    {
        if (set == null || !lookup.ContainsKey(set))
            return;

        RealmMap map = set.Maps.First();
        MapInfo.Value = lookup[set].Maps.First();

        if (!Equals(mapStore.CurrentMapSet, set) || !clock.IsRunning)
            clock.LoadMap(map, true, true);

        clock.RestartPoint = map.Metadata.PreviewTime;
        mapStore.CurrentMapSet = set;
    }

    private void selectMap(RealmMap map)
    {
        if (map == null)
            return;

        menuScroll.Play();
        backgrounds.AddBackgroundFromMap(map);
        selectMapInfo.ChangeMap(map);
        lightController.FadeColour(FluXisColors.GetKeyColor(map.KeyCount), 400);
        ScheduleAfterChildren(() => mapList.ScrollTo(lookup[MapSet.Value]));
    }

    public void Accept()
    {
        if (MapInfo == null)
            return;

        menuAccept.Play();
        backgrounds.AddBackgroundFromMap(MapInfo.Value);
        backgrounds.SwipeAnimation();

        this.Push(new GameplayLoader
        {
            Map = MapInfo.Value,
            Mods = ModSelector.SelectedMods
        });
    }

    private void changeSelection(int by = 0)
    {
        if (maps.Count == 0)
            return;

        int current = maps.IndexOf(MapSet.Value);
        current += by;

        if (current < 0)
            current = maps.Count - 1;
        else if (current >= maps.Count)
            current = 0;

        MapSet.Value = maps[current];
    }

    private void changeMapSelection(int by = 0)
    {
        if (!maps.Contains(MapInfo.Value.MapSet)) return;

        var listEntry = lookup[MapInfo.Value.MapSet];

        int current = listEntry.Maps.IndexOf(MapInfo.Value);
        current += by;

        if (current < 0)
        {
            changeSelection(-1);
            changeMapSelection(MapSet.Value.Maps.Count - 1);
            return;
        }

        if (current >= listEntry.Maps.Count)
        {
            changeSelection(1);
            return;
        }

        MapInfo.Value = listEntry.Maps[current];
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

        if (Equals(set, MapSet.Value))
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
        lightController.FadeColour(FluXisColors.GetKeyColor(MapInfo.Value.KeyCount), 400);
        songSelectBlur.ValueChanged += updateBackgroundBlur;

        mapList.MoveToX(0, 500, Easing.OutQuint);
        selectMapInfo.MoveToX(0, 500, Easing.OutQuint);
        searchBar.Show();
        footer.Show();

        activity.Update("Selecting a map", "", "songselect");

        if (MapInfo.Value != null)
        {
            clock.RestartPoint = MapInfo.Value.Metadata.PreviewTime;

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

        activity.Update("Selecting a map", "", "songselect");

        if (MapInfo.Value != null)
            clock.RestartPoint = MapInfo.Value.Metadata.PreviewTime;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(200);
        clock.Looping = false;

        mapList.MoveToX(-200, 500, Easing.OutQuint);
        selectMapInfo.MoveToX(200, 500, Easing.OutQuint);
        searchBar.Hide();
        footer.Hide();

        mapStore.MapSetAdded -= addMapSet;
        mapStore.MapSetUpdated -= replaceMapSet;
        songSelectBlur.ValueChanged -= updateBackgroundBlur;
        clock.RateTo(1f);

        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.PreviousGroup:
                changeSelection(-1);
                return true;

            case FluXisKeybind.Previous:
                changeMapSelection(-1);
                return true;

            case FluXisKeybind.NextGroup:
                changeSelection(1);
                return true;

            case FluXisKeybind.Next:
                changeMapSelection(1);
                return true;

            case FluXisKeybind.Select:
                Accept();
                return true;

            case FluXisKeybind.Back:
                if (ModSelector.IsOpen.Value)
                {
                    ModSelector.IsOpen.Value = false;
                    return true;
                }

                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

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

        var current = getLetter(MapSet.Value.Metadata.Title[0]);

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

        Logger.Log($"Changing letter to {letter}");

        var first = maps.FirstOrDefault(m => getLetter(m.Metadata.Title.FirstOrDefault(' ')) == letter);
        if (first != null) MapSet.Value = first;

        currentLetter.Text = letter.ToString();
        letterContainer.FadeIn(200).Delay(1000).FadeOut(300);

        if (first == null)
        {
            currentLetter.FadeColour(Colour4.FromHex("#FF5555")).FadeColour(Colour4.White, 1000);
        }
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
        var currentMap = MapSet.Value;

        MapSet.Value = newMap;
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

        MapSet.Value = randomHistory.Last();
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
        MapSet.Value = map.MapSet;
        MapInfo.Value = map;

        var loadedMap = map.GetMapInfo();
        if (loadedMap == null) return;

        var editor = new Editor(map, loadedMap);
        this.Push(editor);
    }
}
