using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Database.Maps;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Select.Footer.Practice;

public partial class FooterPracticeGraph : GridContainer
{
    [Resolved]
    private MapStore maps { get; set; }

    private readonly List<Drawable> bars = new();
    private BindableNumber<int> start { get; }
    private BindableNumber<int> end { get; }

    public FooterPracticeGraph(BindableNumber<int> start, BindableNumber<int> end)
    {
        this.start = start;
        this.end = end;

        const int count = 64;
        var loop = Enumerable.Range(0, count * 2 - 1).ToList();

        ColumnDimensions = loop
                           .Select(x => x % 2 == 0 ? new Dimension() : new Dimension(GridSizeMode.Absolute, 2))
                           .ToArray();

        Content = new[]
        {
            loop.Select(x =>
            {
                if (x % 2 == 0)
                {
                    var bar = new Circle
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Text,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft
                    };

                    bars.Add(bar);
                    return bar;
                }

                return Empty();
            }).ToArray()
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        maps.MapBindable.BindValueChanged(mapChanged, true);

        start.BindValueChanged(updateHighlight, true);
        end.BindValueChanged(updateHighlight, true);
    }

    private void updateHighlight(ValueChangedEvent<int> _) => Scheduler.AddOnce(() =>
    {
        var s = start.Value / (float)end.MaxValue;
        var e = end.Value / (float)end.MaxValue;

        var si = (int)Math.Ceiling(s * bars.Count) - 1;
        var ei = (int)Math.Ceiling(e * bars.Count) - 1;

        for (var i = 0; i < bars.Count; i++)
        {
            var bar = bars[i];
            bar.FadeTo(i >= si && i <= ei ? 1 : .25f, 50);
        }
    });

    private void mapChanged(ValueChangedEvent<RealmMap> v)
    {
        this.FadeTo(.4f, 200);
        Task.Run(() => calculate(v.NewValue));
    }

    private void calculate(RealmMap map)
    {
        var info = map.GetMapInfo();

        if (info is null || info.HitObjects.Count == 0)
            return;

        var count = bars.Count;
        var counters = new float[count];
        var endTime = info.EndTime;

        foreach (var hit in info.HitObjects)
        {
            var progress = hit.Time / endTime;
            var idx = (int)Math.Clamp(Math.Floor(count * progress), 0, count - 1);

            var value = hit.Type switch
            {
                1 => 0.1f, // tick
                _ => 1
            };

            counters[idx] += value;
        }

        var highest = counters.Max();

        Scheduler.ScheduleIfNeeded(() =>
        {
            if (maps.CurrentMap.ID != map.ID)
                return;

            for (var i = 0; i < bars.Count; i++)
            {
                var bar = bars[i];
                bar.ResizeHeightTo(Math.Max(0.02f, counters[i] / highest), 400, Easing.OutQuint);
            }

            this.FadeIn(200);
        });
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapBindable.ValueChanged -= mapChanged;
    }
}
