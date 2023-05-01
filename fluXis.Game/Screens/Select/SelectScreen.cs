using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
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
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Framework.Utils;
using osuTK.Input;

namespace fluXis.Game.Screens.Select;

public partial class SelectScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float Zoom => 1.1f;

    [Resolved]
    private MapStore mapStore { get; set; }

    public BackgroundStack Backgrounds;
    public Bindable<RealmMapSet> MapSet = new();
    public Bindable<RealmMap> MapInfo = new();

    public MapList MapList;
    public SelectMapInfo SelectMapInfo;
    public SearchBar SearchBar;
    public SelectFooter Footer;
    public ModSelector ModSelector;

    public Sample MenuAccept;
    public Sample MenuBack;
    public Sample MenuScroll;

    public readonly List<RealmMapSet> Maps = new();

    public SearchFilters Filters = new();

    private SpriteText noMapsText;

    private readonly Dictionary<RealmMapSet, MapListEntry> lookup = new();

    [BackgroundDependencyLoader]
    private void load(BackgroundStack background, ISampleStore samples)
    {
        Backgrounds = background;

        MenuAccept = samples.Get("UI/accept.mp3");
        MenuBack = samples.Get("UI/back.mp3");
        MenuScroll = samples.Get("UI/scroll.mp3");

        Filters.OnChange += UpdateSearch;

        AddInternal(MapList = new MapList());
        AddInternal(SearchBar = new SearchBar(this));
        AddInternal(SelectMapInfo = new SelectMapInfo { Screen = this });
        AddInternal(Footer = new SelectFooter(this));
        AddInternal(ModSelector = new ModSelector());

        AddInternal(new Container
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            RelativeSizeAxes = Axes.Both,
            Width = .5f,
            Child = noMapsText = new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = "No maps found!",
                Font = FluXisFont.Default(32),
                Blending = BlendingParameters.Additive,
                Alpha = 0
            }
        });

        loadMapSets();

        MapSet.ValueChanged += e => selectMapSet(e.NewValue);
        MapInfo.ValueChanged += e => selectMap(e.NewValue);
    }

    private void loadMapSets()
    {
        var sets = mapStore.MapSetsSorted;

        foreach (RealmMapSet set in sets)
        {
            MapListEntry entry = new(this, set);
            MapList.Add(entry);
            Maps.Add(set);
            lookup[set] = entry;
        }

        if (!sets.Any())
            noMapsText.FadeIn(500);
    }

    protected override void LoadComplete()
    {
        if (Maps.Count > 0)
            MapSet.Value = mapStore.CurrentMapSet;

        mapStore.MapSetAdded += addMapSet;
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

    private void selectMapSet(RealmMapSet set)
    {
        if (set == null)
            return;

        RealmMap map = set.Maps.First();
        MapInfo.Value = lookup[set].Maps.First();

        if (!Equals(mapStore.CurrentMapSet, set) || !Conductor.IsPlaying)
            Conductor.PlayTrack(map, true, map.Metadata.PreviewTime);

        Conductor.SetLoop(map.Metadata.PreviewTime);

        mapStore.CurrentMapSet = set;
    }

    private void selectMap(RealmMap map)
    {
        if (map == null)
            return;

        MenuScroll.Play();
        Backgrounds.AddBackgroundFromMap(map);
        SelectMapInfo.ChangeMap(map);
        MapList.ScrollTo(lookup[MapSet.Value]);
    }

    public void Accept()
    {
        if (MapInfo == null)
            return;

        MenuAccept.Play();
        Backgrounds.AddBackgroundFromMap(MapInfo.Value);
        Backgrounds.SwipeAnimation();

        this.Push(new GameplayScreen(MapInfo.Value, ModSelector.SelectedMods));
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

    private void deleteMap()
    {
        if (MapSet == null)
            return;

        mapStore.DeleteMapSet(MapSet.Value);
        MapList.Remove(lookup[MapSet.Value], false);
        Maps.Remove(MapSet.Value);
        lookup.Remove(MapSet.Value);
        changeSelection(1);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(200);

        MapList.MoveToX(-200, 500, Easing.OutQuint);
        SearchBar.MoveToY(-200, 500, Easing.OutQuint);
        SelectMapInfo.MoveToX(200, 500, Easing.OutQuint);
        Footer.MoveToY(50, 500, Easing.OutQuint);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        this.FadeIn(200);

        MapList.MoveToX(0, 500, Easing.OutQuint);
        SearchBar.MoveToY(0, 500, Easing.OutQuint);
        SelectMapInfo.MoveToX(0, 500, Easing.OutQuint);
        Footer.MoveToY(0, 500, Easing.OutQuint);

        Discord.Update("Selecting a map", "", "songselect");

        if (MapInfo.Value != null)
            Conductor.SetLoop(MapInfo.Value.Metadata.PreviewTime);

        SelectMapInfo.ScoreList.Refresh();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(200);

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
            Conductor.SetLoop(MapInfo.Value.Metadata.PreviewTime);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(200);
        Conductor.SetLoop(0);

        MapList.MoveToX(-200, 500, Easing.OutQuint);
        SearchBar.MoveToY(-200, 500, Easing.OutQuint);
        SelectMapInfo.MoveToX(200, 500, Easing.OutQuint);
        Footer.MoveToY(50, 500, Easing.OutQuint);

        mapStore.MapSetAdded -= addMapSet;
        Conductor.ResetLoop();

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

            case FluXisKeybind.Delete:
                deleteMap();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.F1) mapStore.Export(MapSet.Value);

        return base.OnKeyDown(e);
    }

    public void UpdateSearch()
    {
        Maps.Clear();

        foreach (var child in MapList.Children)
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
}
