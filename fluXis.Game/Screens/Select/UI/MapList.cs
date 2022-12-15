using System;
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
            RelativeSizeAxes = Axes.Both;
            Width = .5f;
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

        public void ScrollTo(MapListEntry entry)
        {
            var pos1 = GetChildPosInContent(entry);
            var pos2 = GetChildPosInContent(entry, entry.DrawSize);

            var min = Math.Min(pos1, pos2);
            var max = Math.Max(pos1, pos2);

            if (min < Current || (min > Current && entry.DrawSize[ScrollDim] > DisplayableContent))
                ScrollTo(min);
            else if (max > Current + DisplayableContent)
                ScrollTo(max - DisplayableContent);
        }
    }
}
