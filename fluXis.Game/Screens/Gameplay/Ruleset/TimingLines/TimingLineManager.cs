using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;

public partial class TimingLineManager : CompositeDrawable
{
    [Resolved]
    private Playfield playfield { get; set; }

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
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        showTimingLines.BindValueChanged(e => this.FadeTo(e.NewValue ? 1 : 0, 400), true);
    }

    public void CreateLines(MapInfo map)
    {
        for (int i = 0; i < map.TimingPoints.Count; i++)
        {
            var point = map.TimingPoints[i];

            if (point.HideLines || point.Signature == 0)
                continue;

            float target = i + 1 < map.TimingPoints.Count ? map.TimingPoints[i + 1].Time : map.EndTime;
            float increase = point.Signature * point.MsPerBeat;
            float position = point.Time;

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
        while (futureTimingLines is { Count: > 0 } && playfield.Manager.ShouldDisplay(futureTimingLines[0].OriginalTime))
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
