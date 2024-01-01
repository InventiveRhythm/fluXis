using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.List;

public partial class TimingPointList : TimingCategoryList<TimingPointList.TimingPointEntry>
{
    public TimingPointList(List<TimingPoint> points, TimingTab tab)
        : base("Timing Points", FluXisColors.Background3, tab)
    {
        points.ForEach(point => AddEntry(new TimingPointEntry(point)));
    }

    public override void OnAdd()
    {
        var point = new TimingPoint
        {
            Time = (float)EditorClock.CurrentTime,
            BPM = 120,
            Signature = 4
        };

        AddEntry(new TimingPointEntry(point));
        EditorValues.MapInfo.Add(point);

        base.OnAdd();
    }

    public override void Sort()
    {
        var entries = GetEntries().ToList();

        entries.Sort((a, b) => a.TimingPoint.Time.CompareTo(b.TimingPoint.Time));
        ReplaceEntries(entries);
    }

    public override void Remove(TimingPointEntry entry)
    {
        EditorValues.MapInfo.Remove(entry.TimingPoint);
        base.Remove(entry);
    }

    public partial class TimingPointEntry : ListEntry
    {
        public readonly TimingPoint TimingPoint;

        private FluXisSpriteText timeText;
        private FluXisSpriteText bpmText;
        private FluXisSpriteText signatureText;

        public TimingPointEntry(TimingPoint timingPoint)
        {
            TimingPoint = timingPoint;
        }

        protected override void Update()
        {
            timeText.Text = TimeUtils.Format(TimingPoint.Time);
            bpmText.Text = TimingPoint.BPM + "bpm";
            signatureText.Text = TimingPoint.Signature + "/4";
            base.Update();
        }

        public override Drawable[] CreateContent() => new Drawable[]
        {
            timeText = new FluXisSpriteText
            {
                Text = TimeUtils.Format(TimingPoint.Time),
                FontSize = 24,
                Width = 100
            },
            bpmText = new FluXisSpriteText
            {
                Text = TimingPoint.BPM + "bpm",
                FontSize = 24,
                Width = 80
            },
            signatureText = new FluXisSpriteText
            {
                Text = TimingPoint.Signature + "/4",
                FontSize = 24
            }
        };

        public override PointSettings CreatePointSettings() => new TimingPointSettings(TimingPoint);

        protected override void Remove() => List.Remove(this);
    }
}
