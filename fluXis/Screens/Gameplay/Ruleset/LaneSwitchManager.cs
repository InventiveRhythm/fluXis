using System;
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

    public bool Mirror { get; init; }

    public bool KeepTransforms { get; init; }
    public override bool RemoveCompletedTransforms => false;

    private List<LaneSwitchEvent> events { get; set; }
    private int keycount { get; set; }
    private bool newLayout { get; set; }
    private bool mirror { get; }

    public LaneSwitchEvent Current { get; private set; }
    public int CurrentCount { get; private set; }
    public float HitPosition { get; private set; }
    private float receptorOffset;
    public float ReceptorOffset 
    { 
        get => receptorOffset;
        private set
        {
            if (receptorOffset != value)
            {
                receptorOffset = value;
                ReceptorOffsetChanged?.Invoke(value);
            }
        }
    }
    
    public event Action<float> ReceptorOffsetChanged;

    public LaneSwitchManager(List<LaneSwitchEvent> events, int keycount, bool newLayout, bool mirror)
    {
        this.newLayout = newLayout;
        this.events = events;
        this.keycount = keycount;
        this.mirror = mirror;
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
        ReceptorOffset = skin.SkinJson.GetKeymode(keycount).ReceptorOffset;

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

            if (Mirror)
                widths.Reverse();

            using (BeginAbsoluteSequence(ls.Time))
            {
                var count = Math.Max(1, ls.Count);
                var pos = (float)skin.SkinJson.GetKeymode(count).HitPosition;
                var rOffset = (float)skin.SkinJson.GetKeymode(count).ReceptorOffset;

                if (first)
                {
                    CurrentCount = count;
                    HitPosition = pos;
                    ReceptorOffset = rOffset;
                }

                var dur = Math.Max(ls.Duration, 0);

                for (int i = 0; i < keycount; i++)
                {
                    var lane = getLane(i + 1);
                    var w = widths[i];

                    if (first)
                        lane.Width = w;

                    lane.ResizeWidthTo(w, dur, ls.Easing);
                }

                this.TransformTo(nameof(HitPosition), pos, dur, ls.Easing);
                this.TransformTo(nameof(ReceptorOffset), rOffset, dur, ls.Easing);
                this.TransformTo(nameof(CurrentCount), count).OnComplete(_ => Current = ls);
            }

            first = false;
        }
    }

    private Drawable getLane(int lane) => InternalChildren[(lane - 1) % keycount];

    private IEnumerable<int> getWidths(LaneSwitchEvent ev)
    {
        var count = Math.Max(1, ev.Count);
        var w = skin.SkinJson.GetKeymode(count).ColumnWidth;
        var row = LaneSwitchEvent.GetRow(count, keycount, newLayout).Select(l => l ? w : 0);

        if (mirror)
            row = row.Reverse();

        return row;
    }
}
