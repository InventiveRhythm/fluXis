using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.List;

public partial class TimingPointList : TimingCategoryList<TimingPointList.TimingPointEntry>
{
    public TimingPointList(List<TimingPointInfo> points, TimingTab tab)
        : base("Timing Points", FluXisColors.Background3, tab)
    {
        points.ForEach(point => AddEntry(new TimingPointEntry(point)));
    }

    public override void OnAdd()
    {
        var point = new TimingPointInfo
        {
            Time = (float)EditorClock.CurrentTime,
            BPM = 120,
            Signature = 4
        };

        AddEntry(new TimingPointEntry(point));
        EditorValues.MapInfo.TimingPoints.Add(point);

        ChangeHandler.OnTimingPointAdded();

        base.OnAdd();
    }

    public override void Sort()
    {
        var entries = GetEntries().ToList();

        entries.Sort((a, b) => a.PointInfo.Time.CompareTo(b.PointInfo.Time));
        ReplaceEntries(entries);
    }

    public override void Remove(TimingPointEntry entry)
    {
        EditorValues.MapInfo.TimingPoints.Remove(entry.PointInfo);
        ChangeHandler.OnTimingPointRemoved();
        base.Remove(entry);
    }

    public partial class TimingPointEntry : ListEntry
    {
        public readonly TimingPointInfo PointInfo;

        private FluXisSpriteText timeText;
        private FluXisSpriteText bpmText;
        private FluXisSpriteText signatureText;

        public TimingPointEntry(TimingPointInfo pointInfo)
        {
            PointInfo = pointInfo;
        }

        protected override void Update()
        {
            timeText.Text = TimeUtils.Format(PointInfo.Time);
            bpmText.Text = PointInfo.BPM + "bpm";
            signatureText.Text = PointInfo.Signature + "/4";
            base.Update();
        }

        public override Drawable[] CreateContent() => new Drawable[]
        {
            timeText = new FluXisSpriteText
            {
                Text = TimeUtils.Format(PointInfo.Time),
                FontSize = 24,
                Width = 100
            },
            bpmText = new FluXisSpriteText
            {
                Text = PointInfo.BPM + "bpm",
                FontSize = 24,
                Width = 80
            },
            signatureText = new FluXisSpriteText
            {
                Text = PointInfo.Signature + "/4",
                FontSize = 24
            }
        };

        public override PointSettings CreatePointSettings() => new TimingPointSettings(PointInfo);

        protected override void Remove() => List.Remove(this);
    }
}
