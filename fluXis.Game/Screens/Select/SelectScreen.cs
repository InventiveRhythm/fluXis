using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Activity;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Context;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notification;
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

public partial class SelectScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float Zoom => 1.1f;
    public override float BackgroundBlur => songSelectBlur.Value ? 0.25f : 0;
    public override bool ApplyValuesAfterLoad => true;

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [Resolved]
    private LightController lightController { get; set; }

    [Resolved]
    private AudioClock clock { get; set; }

    [Resolved]
    private ActivityManager activity { get; set; }

    public BackgroundStack Backgrounds;
    public Bindable<RealmMapSet> MapSet = new();
    public Bindable<RealmMap> MapInfo = new();
    public readonly List<RealmMapSet> Maps = new();
    public SearchFilters Filters = new();

    public MapList MapList;
    public SelectMapInfo SelectMapInfo;
    public SearchBar SearchBar;
    public SelectFooter Footer;
    public ModSelector ModSelector;

    public Sample MenuAccept;
    public Sample MenuBack;
    public Sample MenuScroll;

    private FluXisSpriteText noMapsText;
    private LoadingIcon loadingIcon;
    private Container letterContainer;
    private FluXisSpriteText currentLetter;

    private readonly Dictionary<RealmMapSet, MapListEntry> lookup = new();

    private Bindable<bool> songSelectBlur;

    [BackgroundDependencyLoader]
    private void load(BackgroundStack background, ISampleStore samples, FluXisConfig config)
    {
        Backgrounds = background;

        MenuAccept = samples.Get("UI/accept.mp3");
        MenuBack = samples.Get("UI/back.mp3");
        MenuScroll = samples.Get("UI/scroll.mp3");

        Filters.OnChange += UpdateSearch;
        songSelectBlur = config.GetBindable<bool>(FluXisSetting.SongSelectBlur);

        InternalChildren = new Drawable[]
        {
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
                        Padding = new MarginPadding(10) { Top = 60, Left = 20 },
                        Child = MapList = new MapList { Alpha = 0 }
                    },
                    SearchBar = new SearchBar(this),
                    SelectMapInfo = new SelectMapInfo { Screen = this },
                    new Container
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.Both,
                        Width = .5f,
                        Children = new Drawable[]
                        {
                            noMapsText = new FluXisSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = "No maps found!",
                                FontSize = 32,
                                Blending = BlendingParameters.Additive,
                                Alpha = 0
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
                                Origin = Anchor.Centre
                            }
                        }
                    },
                }
            },
            Footer = new SelectFooter { Screen = this },
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
            Schedule(() => MapList.AddMap(entry));
            Maps.Add(set);
            lookup[set] = entry;
        }

        if (!sets.Any()) Schedule(() => noMapsText.FadeIn(500));
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
                if (Maps.Count > 0)
                    MapSet.Value = mapStore.CurrentMapSet;

                MapList.FadeIn(500);
                loadingIcon.FadeOut(500);
            });
        });
    }

    private void addMapSet(RealmMapSet set)
    {
        Schedule(() =>
        {
            int index = mapStore.MapSetsSorted.IndexOf(set);

            MapListEntry entry = new(this, set);
            Maps.Insert(index, set);
            MapList.Insert(index, entry);
            lookup[set] = entry;

            MapSet.Value = set;

            noMapsText.FadeOut(200);
        });
    }

    private void replaceMapSet(RealmMapSet oldSet, RealmMapSet newSet)
    {
        void action()
        {
            if (!lookup.ContainsKey(oldSet))
            {
                Schedule(action); // do this in next frame
                return;
            }

            MapList.Remove(lookup[oldSet], false);
            Maps.Remove(oldSet);
            lookup.Remove(oldSet);
            changeSelection(1);
            addMapSet(newSet);
            Schedule(() => { Schedule(() => selectMapSet(newSet)); }); // <- this looks stupid and probably is, but it works
        }

        Schedule(action);
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

        MenuScroll.Play();
        Backgrounds.AddBackgroundFromMap(map);
        SelectMapInfo.ChangeMap(map);
        lightController.FadeColour(FluXisColors.GetKeyColor(map.KeyCount), 400);
        ScheduleAfterChildren(() => MapList.ScrollTo(lookup[MapSet.Value]));
    }

    public void Accept()
    {
        if (MapInfo == null)
            return;

        MenuAccept.Play();
        Backgrounds.AddBackgroundFromMap(MapInfo.Value);
        Backgrounds.SwipeAnimation();

        this.Push(new GameplayLoader
        {
            Map = MapInfo.Value,
            Mods = ModSelector.SelectedMods
        });
    }

    private void changeSelection(int by = 0)
    {
        if (Maps.Count == 0)
            return;

        int current = Maps.IndexOf(MapSet.Value);
        current += by;

        if (current < 0)
            current = Maps.Count - 1;
        else if (current >= Maps.Count)
            current = 0;

        MapSet.Value = Maps[current];
    }

    private void changeMapSelection(int by = 0)
    {
        if (!Maps.Contains(MapInfo.Value.MapSet)) return;

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

    public void DeleteMapSet(RealmMapSet set)
    {
        if (set == null)
            return;

        mapStore.DeleteMapSet(set);
        MapList.Remove(lookup[set], false);
        Maps.Remove(set);
        lookup.Remove(set);

        if (Equals(set, MapSet.Value))
            changeSelection(1);

        if (Maps.Count == 0)
            noMapsText.FadeIn(500);
    }

    public void ExportMapSet(RealmMapSet set)
    {
        if (set == null)
            return;

        LoadingNotification notification = new LoadingNotification
        {
            TextLoading = $"Exporting mapset {set.Metadata.Artist} - {set.Metadata.Title}...",
            TextSuccess = $"Exported mapset {set.Metadata.Artist} - {set.Metadata.Title}!",
            TextFailure = $"Failed to export mapset {set.Metadata.Artist} - {set.Metadata.Title}!",
            ShowProgress = true
        };

        notifications.AddNotification(notification);
        Task.Run(() => mapStore.Export(set, notification));
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(200);
        songSelectBlur.ValueChanged -= updateBackgroundBlur;

        MapList.MoveToX(-200, 500, Easing.OutQuint);
        SearchBar.MoveToY(-200, 500, Easing.OutQuint);
        SelectMapInfo.MoveToX(200, 500, Easing.OutQuint);
        Footer.MoveToY(50, 500, Easing.OutQuint);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        this.FadeIn(200);
        lightController.FadeColour(FluXisColors.GetKeyColor(MapInfo.Value.KeyCount), 400);
        songSelectBlur.ValueChanged += updateBackgroundBlur;

        MapList.MoveToX(0, 500, Easing.OutQuint);
        SearchBar.MoveToY(0, 500, Easing.OutQuint);
        SelectMapInfo.MoveToX(0, 500, Easing.OutQuint);
        Footer.MoveToY(0, 500, Easing.OutQuint);

        activity.Update("Selecting a map", "", "songselect");

        if (MapInfo.Value != null)
        {
            clock.RestartPoint = MapInfo.Value.Metadata.PreviewTime;

            if (!clock.IsRunning)
                clock.Start();
        }

        SelectMapInfo.ScoreList.Refresh();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(200);
        songSelectBlur.ValueChanged += updateBackgroundBlur;

        MapList.MoveToX(-200)
               .MoveToX(0, 500, Easing.OutQuint);

        SearchBar.MoveToY(-200)
                 .MoveToY(0, 500, Easing.OutQuint);

        SelectMapInfo.MoveToX(200)
                     .MoveToX(0, 500, Easing.OutQuint);

        Footer.MoveToY(50)
              .MoveToY(0, 500, Easing.OutQuint);

        activity.Update("Selecting a map", "", "songselect");

        if (MapInfo.Value != null)
            clock.RestartPoint = MapInfo.Value.Metadata.PreviewTime;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(200);
        clock.Looping = false;

        MapList.MoveToX(-200, 500, Easing.OutQuint);
        SearchBar.MoveToY(-200, 500, Easing.OutQuint);
        SelectMapInfo.MoveToX(200, 500, Easing.OutQuint);
        Footer.MoveToY(50, 500, Easing.OutQuint);

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

                MenuBack.Play();
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
            case Key.F1:
                ModSelector.IsOpen.Toggle();
                return true;

            case Key.F2:
                RandomMap();
                return true;

            case Key.F3:
                Footer.OpenSettings();
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

    protected override bool OnJoystickPress(JoystickPressEvent e)
    {
        Logger.Log($"Joystick button {e.Button} pressed");

        switch (e.Button)
        {
            case JoystickButton.Button1: // X
                ModSelector.IsOpen.Toggle();
                return true;

            case JoystickButton.Button2: // A
                Accept();
                return true;

            case JoystickButton.Button3: // B
                this.Exit();
                return true;

            case JoystickButton.Button4: // Y
                RandomMap();
                return true;

            case JoystickButton.GamePadLeftShoulder: // LB
                return true;

            case JoystickButton.GamePadRightShoulder: // RB
                return true;

            case JoystickButton.Hat1Up or JoystickButton.Axis2Negative: // Up
                changeMapSelection(-1);
                return true;

            case JoystickButton.Hat1Down or JoystickButton.Axis2Positive: // Down
                changeMapSelection(1);
                return true;

            case JoystickButton.Hat1Left or JoystickButton.FirstAxisNegative: // Left
                changeSelection(-1);
                return true;

            case JoystickButton.Hat1Right or JoystickButton.FirstAxisPositive: // Right
                changeSelection(1);
                return true;

            case JoystickButton.Button9: // Back
                return true;

            case JoystickButton.Button10: // Start
                Footer.OpenSettings();
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

        var current = getLetter(MapSet.Value.Metadata.Title[0]);

        var index = Array.IndexOf(letters, current);
        index += by;

        if (index < 0) index = letters.Length - 1;
        if (index >= letters.Length) index = 0;

        while (Maps.All(m => getLetter(m.Metadata.Title[0]) != letters[index]))
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

        Logger.Log($"Changing letter to {letter}");

        var first = Maps.FirstOrDefault(m => getLetter(m.Metadata.Title[0]) == letter);
        if (first != null) MapSet.Value = first;

        currentLetter.Text = letter.ToString();
        letterContainer.FadeIn(200).Delay(1000).FadeOut(300);

        if (first == null)
        {
            currentLetter.FadeColour(Colour4.FromHex("#FF5555")).FadeColour(Colour4.White, 1000);
        }
    }

    public void UpdateSearch()
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
            noMapsText.FadeIn(200);
        else
            noMapsText.FadeOut(200);
    }

    public void RandomMap()
    {
        if (Maps.Count == 0)
            return;

        MapSet.Value = Maps[RNG.Next(0, Maps.Count)];
    }

    private void updateBackgroundBlur(ValueChangedEvent<bool> e)
    {
        if (e.NewValue)
            Backgrounds.SetBlur(BackgroundBlur, 500);
        else
            Backgrounds.SetBlur(0, 500);
    }
}
