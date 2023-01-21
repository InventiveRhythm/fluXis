using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Select.UI;
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
    public class SelectScreen : Screen, IKeyBindingHandler<FluXisKeybind>
    {
        public BackgroundStack Backgrounds;
        public MapStore MapStore;
        public MapSet MapSet;
        public MapInfo MapInfo;

        public MapList MapList;

        public Sample MenuAccept;
        public Sample MenuBack;
        public Sample MenuScroll;

        private SpriteText noMapsText;

        private readonly Dictionary<MapSet, MapListEntry> lookup = new Dictionary<MapSet, MapListEntry>();

        [BackgroundDependencyLoader]
        private void load(MapStore maps, BackgroundStack background, ISampleStore samples)
        {
            Backgrounds = background;
            MapStore = maps;

            MenuAccept = samples.Get("ui/accept.ogg");
            MenuBack = samples.Get("ui/back.ogg");
            MenuScroll = samples.Get("ui/scroll.ogg");

            AddInternal(MapList = new MapList());
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

            if (MapStore.MapSets.Count == 0)
            {
                noMapsText.Alpha = 1;
            }
            else
            {
                int i = 0;

                foreach (var set in MapStore.MapSets)
                {
                    lookup[set] = MapList.AddMap(this, set, i);
                    i++;
                }
            }
        }

        protected override void LoadComplete()
        {
            if (MapStore.MapSets.Count > 0)
                SelectMapSet(MapStore.CurrentMapSet);
        }

        public void SelectMapSet(MapSet set)
        {
            if (set == null)
                return;

            MapInfo map = set.Maps.First();
            MapSet = set;
            MapInfo = map;
            SelectMap(map);
            Conductor.SetLoop(map.Metadata.PreviewTime);
            MapList.ScrollTo(lookup[set]);

            if (MapStore.CurrentMapSet != set || !Conductor.IsPlaying)
                Conductor.PlayTrack(map, true, map.Metadata.PreviewTime);

            MapStore.CurrentMapSet = set;
        }

        public void SelectMap(MapInfo map)
        {
            if (map == null)
                return;

            MapInfo = map;
            MenuScroll.Play();
            Backgrounds.AddBackgroundFromMap(map);
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
            if (MapStore.MapSets.Count == 0)
                return;

            int current = MapStore.MapSets.IndexOf(MapSet);
            current += by;

            if (current < 0)
                current = MapStore.MapSets.Count - 1;
            else if (current >= MapStore.MapSets.Count)
                current = 0;

            SelectMapSet(MapStore.MapSets[current]);
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

        public override void OnSuspending(ScreenTransitionEvent e)
        {
            this.FadeOut(500);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            this.FadeIn(500);

            Discord.Update("Selecting a map", "", "songselect");
            Conductor.SetLoop(MapInfo.Metadata.PreviewTime);

            base.OnResuming(e);
        }

        public override void OnEntering(ScreenTransitionEvent e)
        {
            this.FadeInFromZero(500);
            Discord.Update("Selecting a map", "", "songselect");

            base.OnEntering(e);
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            this.FadeOut(500);

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
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
    }
}
