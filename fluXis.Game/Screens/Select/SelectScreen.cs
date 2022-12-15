using System.Linq;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens.Select.UI;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Select
{
    public class SelectScreen : Screen
    {
        public BackgroundStack Backgrounds;

        [BackgroundDependencyLoader]
        private void load(MapStore maps, BackgroundStack background)
        {
            Backgrounds = background;

            MapList list;
            AddInternal(list = new MapList());

            int i = 0;

            foreach (var set in maps.GetMapSets())
            {
                list.AddMap(this, set.Maps.First(), i);
                i++;
            }

            Backgrounds.AddBackgroundFromMap(maps.GetMapSets().First().Maps.First());
        }
    }
}
