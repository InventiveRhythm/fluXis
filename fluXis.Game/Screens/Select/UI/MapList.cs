using System.Linq;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Select.UI
{
    public class MapList : BasicScrollContainer
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            RelativeSizeAxes = Axes.X;
            Width = .5f;
            Height = 75;
            Masking = false;
            Padding = new MarginPadding(10);

            Scrollbar.CornerRadius = 5;
            Scrollbar.Width = 10;
            Scrollbar.Colour = Colour4.White;
        }

        public MapListEntry AddMap(SelectScreen screen, MapSet map, int index)
        {
            var entry = new MapListEntry(screen, map, index);
            entry.Y = Content.Children.Count > 0 ? Content.Children.Last().Y + Content.Children.Last().Height + 5 : 0;
            Content.Add(entry);
            return entry;
        }
    }
}
