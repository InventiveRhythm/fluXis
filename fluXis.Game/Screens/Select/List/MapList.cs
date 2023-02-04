using System;
using System.Linq;
using fluXis.Game.Database.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Select.List
{
    public partial class MapList : BasicScrollContainer
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            RelativeSizeAxes = Axes.Both;
            Width = .5f;
            Masking = false;
            Padding = new MarginPadding(10);
        }

        public MapListEntry AddMap(SelectScreen screen, RealmMapSet map, int index)
        {
            var entry = new MapListEntry(screen, map, index);
            entry.Y = Content.Children.Count > 0 ? Content.Children.Last().Y + Content.Children.Last().Height + 5 : 0;
            Content.Add(entry);
            return entry;
        }

        protected override void Update()
        {
            for (var i = 0; i < Content.Children.Count; i++)
            {
                var child = Content.Children[i];

                if (i > 0)
                    child.Y = Content.Children[i - 1].Y + Content.Children[i - 1].Height + 5;
                else
                    child.Y = 0;
            }

            base.Update();
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

        protected override ScrollbarContainer CreateScrollbar(Direction direction) => new MapListScrollbar(direction);

        protected partial class MapListScrollbar : ScrollbarContainer
        {
            public MapListScrollbar(Direction direction)
                : base(direction)
            {
                // the fuck did i do its just gone
                CornerRadius = 5;
                Margin = new MarginPadding(2);
                Masking = true;
                Size = new Vector2(10);
                Colour = Colour4.White;
                Anchor = Origin = Anchor.TopLeft;
            }

            public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
            {
                Vector2 size = new Vector2(10)
                {
                    [(int)ScrollDirection] = val
                };
                this.ResizeTo(size, duration, easing);
            }
        }
    }
}
