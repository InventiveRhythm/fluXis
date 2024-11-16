using System.Collections.Generic;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsLaneMask : CompositeDrawable
{
    private EditorMap map { get; }
    private IHasLaneMask mask { get; }

    public PointSettingsLaneMask(EditorMap map, IHasLaneMask mask)
    {
        this.map = map;
        this.mask = mask;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        while (mask.LaneMask.Count < map.RealmMap.KeyCount)
            mask.LaneMask.Add(true);

        var bind = new BindableList<bool>(mask.LaneMask);

        var draws = new List<Drawable>();
        var cols = new List<Dimension>();

        for (int i = 0; i < map.RealmMap.KeyCount; i++)
        {
            var idx = i;

            draws.AddRange(new[]
            {
                Empty(),
                new LaneButton(bind, i)
                {
                    Action = () =>
                    {
                        var cur = mask.ValidFor(idx + 1);
                        mask.LaneMask[idx] = bind[idx] = !cur;
                    }
                }
            });

            cols.AddRange(new Dimension[]
            {
                new(GridSizeMode.Absolute, 8),
                new()
            });
        }

        draws.RemoveAt(0);
        cols.RemoveAt(0);

        InternalChildren = new Drawable[]
        {
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = cols.ToArray(),
                Content = new[] { draws.ToArray() }
            }
        };
    }

    private partial class LaneButton : ClickableContainer
    {
        private BindableList<bool> list { get; }
        private int idx { get; }

        private bool active => list.Count < idx + 1 || list[idx];

        public LaneButton(BindableList<bool> l, int i)
        {
            list = l;
            idx = i;

            RelativeSizeAxes = Axes.Both;
            CornerRadius = 8;
            Masking = true;

            var box = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = active ? FluXisColors.Highlight : FluXisColors.Background3
            };

            Child = box;

            list.CollectionChanged += (_, _) => box.Colour = active ? FluXisColors.Highlight : FluXisColors.Background3;
        }
    }
}
