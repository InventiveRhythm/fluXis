using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Screens.Select.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Select
{
    public class SelectScreen : Screen, IKeyBindingHandler<FluXisKeybind>
    {
        public BackgroundStack Backgrounds;
        private List<MapSet> mapSets;
        public MapSet MapSet;
        public MapInfo MapInfo;

        public MapList MapList;

        public Sample MenuAccept;
        public Sample MenuBack;
        public Sample MenuScroll;

        private readonly Dictionary<MapSet, MapListEntry> lookup = new Dictionary<MapSet, MapListEntry>();

        [BackgroundDependencyLoader]
        private void load(MapStore maps, BackgroundStack background, ISampleStore samples)
        {
            Backgrounds = background;
            mapSets = maps.GetMapSets();

            MenuAccept = samples.Get("ui/accept.ogg");
            MenuBack = samples.Get("ui/back.ogg");
            MenuScroll = samples.Get("ui/scroll.ogg");

            AddInternal(MapList = new MapList());

            int i = 0;

            foreach (var set in mapSets)
            {
                lookup[set] = MapList.AddMap(this, set, i);
                i++;
            }

            if (MapSet != null)
                return;

            SelectMapSet(mapSets.First());
        }

        public void SelectMapSet(MapSet set)
        {
            MapInfo map = set.Maps.First();
            MapSet = set;
            Backgrounds.AddBackgroundFromMap(map);
            Conductor.PlayTrack(map, true, map.Metadata.PreviewTime);
            Conductor.SetLoop(map.Metadata.PreviewTime);
            MenuScroll.Play();
            MapList.ScrollTo(lookup[set]);
        }

        private void changeSelection(int by = 0)
        {
            int current = mapSets.IndexOf(MapSet);
            current += by;

            if (current < 0)
                current = mapSets.Count - 1;
            else if (current >= mapSets.Count)
                current = 0;

            SelectMapSet(mapSets[current]);
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

        public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
        {
            switch (e.Action)
            {
                case FluXisKeybind.PreviousGroup:
                    changeSelection(-1);
                    return true;

                case FluXisKeybind.NextGroup:
                    changeSelection(1);
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
    }
}
