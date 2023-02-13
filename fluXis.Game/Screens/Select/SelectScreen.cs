using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Select.Info;
using fluXis.Game.Screens.Select.List;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Select
{
    public partial class SelectScreen : Screen, IKeyBindingHandler<FluXisKeybind>
    {
        [Resolved]
        private MapStore maps { get; set; }

        public BackgroundStack Backgrounds;
        public RealmMapSet MapSet;
        public RealmMap MapInfo;

        public MapList MapList;
        public SelectMapInfo SelectMapInfo;

        public Sample MenuAccept;
        public Sample MenuBack;
        public Sample MenuScroll;

        private SpriteText noMapsText;

        private readonly Dictionary<RealmMapSet, MapListEntry> lookup = new();

        [BackgroundDependencyLoader]
        private void load(BackgroundStack background, ISampleStore samples)
        {
            Backgrounds = background;

            MenuAccept = samples.Get("ui/accept.ogg");
            MenuBack = samples.Get("ui/back.ogg");
            MenuScroll = samples.Get("ui/scroll.ogg");

            AddInternal(MapList = new MapList());
            AddInternal(SelectMapInfo = new SelectMapInfo());
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
                    Font = new FontUsage("Quicksand", 32, "Bold"),
                    Blending = BlendingParameters.Additive,
                    Alpha = 0
                }
            });

            loadMapSets();
        }

        private void loadMapSets()
        {
            var sets = maps.MapSets;

            int i = 0;

            foreach (RealmMapSet set in sets)
            {
                MapListEntry entry = new(this, set, i);
                MapList.Add(entry);
                lookup[set] = entry;
                i++;
            }

            if (!sets.Any())
                noMapsText.FadeIn(500);
        }

        protected override void LoadComplete()
        {
            if (maps.MapSets.Count > 0)
                SelectMapSet(maps.CurrentMapSet);
        }

        public void SelectMapSet(RealmMapSet set)
        {
            if (set == null)
                return;

            RealmMap map = set.Maps.First();
            MapSet = set;
            MapInfo = map;
            SelectMap(map);
            MapList.ScrollTo(lookup[set]);

            if (!Equals(maps.CurrentMapSet, set) || !Conductor.IsPlaying)
                Conductor.PlayTrack(map, true, map.Metadata.PreviewTime);

            Conductor.SetLoop(map.Metadata.PreviewTime);

            maps.CurrentMapSet = set;
        }

        public void SelectMap(RealmMap map)
        {
            if (map == null)
                return;

            MapInfo = map;
            MenuScroll.Play();
            Backgrounds.AddBackgroundFromMap(map);
            SelectMapInfo.ChangeMap(map);
            MapList.ScrollTo(lookup[MapSet]);
        }

        public void Accept()
        {
            if (MapInfo == null)
                return;

            MenuAccept.Play();
            Backgrounds.AddBackgroundFromMap(MapInfo);
            Backgrounds.SwipeAnimation();

            this.Push(new GameplayScreen(MapInfo));
        }

        private void changeSelection(int by = 0)
        {
            if (maps.MapSets.Count == 0)
                return;

            int current = maps.MapSets.IndexOf(MapSet);
            current += by;

            if (current < 0)
                current = maps.MapSets.Count - 1;
            else if (current >= maps.MapSets.Count)
                current = 0;

            SelectMapSet(maps.MapSets[current]);
        }

        private void changeMapSelection(int by = 0)
        {
            int current = MapSet.Maps.IndexOf(MapInfo);
            current += by;

            if (current < 0)
            {
                changeSelection(-1);
                changeMapSelection(MapSet.Maps.Count - 1);
                return;
            }

            if (current >= MapSet.Maps.Count)
            {
                changeSelection(1);
                return;
            }

            SelectMap(MapSet.Maps[current]);
        }

        private void deleteMap()
        {
            if (MapSet == null)
                return;

            maps.DeleteMapSet(MapSet);
            MapList.Remove(lookup[MapSet], false);
            lookup.Remove(MapSet);
            changeSelection(1);
        }

        public override void OnSuspending(ScreenTransitionEvent e)
        {
            this.FadeOut(200);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            this.FadeIn(200);

            Discord.Update("Selecting a map", "", "songselect");
            Conductor.SetLoop(MapInfo.Metadata.PreviewTime);
            SelectMapInfo.ScoreList.Refresh();

            base.OnResuming(e);
        }

        public override void OnEntering(ScreenTransitionEvent e)
        {
            this.FadeInFromZero(200);
            Discord.Update("Selecting a map", "", "songselect");
            Conductor.SetLoop(MapInfo.Metadata.PreviewTime);

            base.OnEntering(e);
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            this.FadeOut(200);
            Conductor.SetLoop(0);

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
    }
}
