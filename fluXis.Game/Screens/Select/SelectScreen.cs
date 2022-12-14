using System.Linq;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens.Select.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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

            FillFlowContainer list;
            AddInternal(list = new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.Both,
                Width = .5f,
                Masking = true,
                Padding = new MarginPadding(10),
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
            });

            foreach (var map in maps.GetMaps()) list.Add(new MapListEntry(this, map));
            Backgrounds.AddBackgroundFromMap(maps.GetMaps().First());
        }
    }
}
