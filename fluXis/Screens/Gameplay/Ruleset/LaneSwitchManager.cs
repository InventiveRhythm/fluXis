using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures.Events;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset;

public partial class LaneSwitchManager : CompositeComponent
{
    [Resolved]
    private ISkin skin { get; set; }

    public bool KeepTransforms { get; init; }
    public override bool RemoveCompletedTransforms => !KeepTransforms;

    private List<LaneSwitchEvent> events { get; set; }
    private int keycount { get; set; }
    private bool newLayout { get; set; }

    public LaneSwitchEvent Current { get; private set; }
    public int CurrentCount { get; private set; }
    public float HitPosition { get; private set; }

    public LaneSwitchManager(List<LaneSwitchEvent> events, int keycount, bool newLayout)
    {
        this.newLayout = newLayout;
        this.events = events;
        this.keycount = keycount;
    }

    [BackgroundDependencyLoader]
    private void load() => build();

    public void Rebuild(List<LaneSwitchEvent> events, int keycount, bool newLayout)
    {
        this.events = events;
        this.keycount = keycount;
        this.newLayout = newLayout;
        build();
    }

    public float WidthFor(int lane) => getLane(lane).Width;

    private void build()
    {
        ClearTransforms(true);
        ClearInternal();

        CurrentCount = keycount;
        HitPosition = skin.SkinJson.GetKeymode(keycount).HitPosition;

        for (int i = 0; i < keycount; i++)
        {
            var draw = Empty();
            draw.Width = skin.SkinJson.GetKeymode(keycount).ColumnWidth;
            AddInternal(draw);
        }

        var first = true;

        foreach (var ls in events)
        {
            var widths = getWidths(ls).ToList();

            using (BeginAbsoluteSequence(ls.Time))
            {
                var pos = (float)skin.SkinJson.GetKeymode(ls.Count).HitPosition;

                if (first)
                {
                    CurrentCount = ls.Count;
                    HitPosition = pos;
                }

                for (int i = 0; i < keycount; i++)
                {
                    var lane = getLane(i + 1);
                    var w = widths[i];

                    if (first)
                        lane.Width = w;

                    lane.ResizeWidthTo(w, ls.Duration, ls.Easing);
                }

                this.TransformTo(nameof(HitPosition), pos, ls.Duration, ls.Easing);
                this.TransformTo(nameof(CurrentCount), ls.Count).OnComplete(_ => Current = ls);
            }

            first = false;
        }
    }

    private Drawable getLane(int lane) => InternalChildren[lane - 1];

    private IEnumerable<int> getWidths(LaneSwitchEvent ev)
    {
        var w = skin.SkinJson.GetKeymode(ev.Count).ColumnWidth;
        return LaneSwitchEvent.GetRow(ev.Count, keycount, newLayout).Select(l => l ? w : 0);
    }
}
