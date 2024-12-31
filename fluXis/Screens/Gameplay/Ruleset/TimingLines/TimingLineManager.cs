using System.Collections.Generic;
using System.Linq;
using fluXis.Configuration;
using fluXis.Map;
using fluXis.Screens.Gameplay.Ruleset.HitObjects;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset.TimingLines;

public partial class TimingLineManager : CompositeDrawable
{
    [Resolved]
    private Playfield playfield { get; set; }

    private HitObjectColumn column => playfield.Manager[0];

    private Bindable<bool> showTimingLines;

    private readonly List<TimingLine> timingLines = new();
    private readonly List<TimingLine> futureTimingLines = new();

    public TimingLineManager()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Masking = true;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        showTimingLines = config.GetBindable<bool>(FluXisSetting.TimingLines);
        createLines(playfield.Map);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        showTimingLines.BindValueChanged(e => this.FadeTo(e.NewValue ? 1 : 0, 400), true);
    }

    private void createLines(MapInfo map)
    {
        for (int i = 0; i < map.TimingPoints.Count; i++)
        {
            var point = map.TimingPoints[i];

            if (point.HideLines || point.Signature == 0)
                continue;

            var target = i + 1 < map.TimingPoints.Count ? map.TimingPoints[i + 1].Time : map.EndTime;
            var increase = point.Signature * point.MsPerBeat;
            var position = point.Time;

            while (position < target)
            {
                futureTimingLines.Add(new TimingLine(position));
                position += increase;
            }
        }

        futureTimingLines.Sort((a, b) => a.OriginalTime.CompareTo(b.OriginalTime));
    }

    protected override void Update()
    {
        while (futureTimingLines is { Count: > 0 } && column.ShouldDisplay(futureTimingLines[0].OriginalTime))
        {
            TimingLine line = futureTimingLines[0];
            futureTimingLines.RemoveAt(0);
            timingLines.Add(line);
            AddInternal(line);
        }

        foreach (var line in timingLines.Where(t => t.Y > DrawHeight).ToArray())
        {
            timingLines.Remove(line);
            RemoveInternal(line, true);
        }

        base.Update();
    }
}
