using System.Collections.Generic;
using fluXis.Game.Map;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;

public partial class TimingLineManager : CompositeDrawable
{
    public HitObjectManager HitObjectManager { get; }

    private readonly List<TimingLine> timingLines = new();
    private readonly List<TimingLine> futureTimingLines = new();

    public TimingLineManager(HitObjectManager hitObjectManager)
    {
        HitObjectManager = hitObjectManager;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
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
                futureTimingLines.Add(new TimingLine(this, HitObjectManager.PositionFromTime(position)));
                position += increase;
            }
        }

        futureTimingLines.Sort((a, b) => a.ScrollVelocityTime.CompareTo(b.ScrollVelocityTime));
    }

    protected override void Update()
    {
        while (futureTimingLines is { Count: > 0 } && futureTimingLines[0].ScrollVelocityTime <= HitObjectManager.CurrentTime + 2000 * HitObjectManager.ScrollSpeed)
        {
            TimingLine line = futureTimingLines[0];
            futureTimingLines.RemoveAt(0);
            timingLines.Add(line);
            AddInternal(line);
        }

        Width = HitObjectManager.Playfield.Stage.Width;
        base.Update();
    }
}
