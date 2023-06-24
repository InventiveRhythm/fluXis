using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Utils;
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
                            loadingIcon = new LoadingIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            }
                        }
                    },
                }
            },
            Footer = new SelectFooter(this),
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
        Schedule(() =>
        {
            MapList.Remove(lookup[oldSet], false);
            Maps.Remove(oldSet);
            lookup.Remove(oldSet);
            changeSelection(1);
            addMapSet(newSet);
            Schedule(() => { Schedule(() => selectMapSet(newSet)); }); // <- this looks stupid and probably is, but it works
        });
    }

    private void selectMapSet(RealmMapSet set)
    {
        if (set == null)
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

        Discord.Update("Selecting a map", "", "songselect");

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

        Discord.Update("Selecting a map", "", "songselect");

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
        }

        return false;
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
