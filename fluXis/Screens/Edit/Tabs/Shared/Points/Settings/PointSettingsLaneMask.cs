using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsLaneMask : PointSettingsBase
{
    private EditorMap map { get; }
    private IHasGroups groups { get; }

    public PointSettingsLaneMask(EditorMap map, IHasGroups groups)
    {
        this.map = map;
        this.groups = groups;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        if (groups.Groups.Count == 0)
        {
            for (int i = 0; i < map.RealmMap.KeyCount; i++)
                groups.Groups.Add($"${i + 1}");
        }

        var bind = new BindableList<string>(groups.Groups);

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
                        var group = $"${idx + 1}";

                        if (groups.Groups.Contains(group))
                            bind.Remove(group);
                        else
                            bind.Add(group);

                        var list = bind.ToList();

                        // to save space, we can clear the list
                        // since empty lists get applies to all lanes anyway
                        if (list.Count == map.RealmMap.KeyCount && list.All(x => x.StartsWith('$')))
                            list.Clear();

                        groups.Groups = list;
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
        private BindableList<string> list { get; }
        private int idx { get; }

        private bool active => list.Contains($"${idx + 1}");

        public LaneButton(BindableList<string> l, int i)
        {
            list = l;
            idx = i;

            RelativeSizeAxes = Axes.Both;
            CornerRadius = 8;
            Masking = true;

            var box = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = active ? Theme.Highlight : Theme.Background3
            };

            Child = box;

            list.CollectionChanged += (_, _) => box.Colour = active ? Theme.Highlight : Theme.Background3;
        }
    }
}
