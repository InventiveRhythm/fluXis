using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings;
using fluXis.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.List;

public partial class TimingPointList : TimingCategoryList<TimingPointList.TimingPointEntry>
{
    public TimingPointList(List<TimingPointInfo> points, TimingTab tab)
        : base("Timing Points", FluXisColors.Surface, tab)
    {
        points.ForEach(point => AddEntry(new TimingPointEntry(point)));
    }

    public override void OnAdd()
    {
        AddEntry(new TimingPointEntry(new TimingPointInfo
        {
            Time = Conductor.CurrentTime,
            BPM = 120,
            Signature = 4
        }));

        TimingTab.OnTimingPointChanged();

        base.OnAdd();
    }

    public override void Sort()
    {
        var entries = GetEntries().ToList();

        entries.Sort((a, b) => a.PointInfo.Time.CompareTo(b.PointInfo.Time));
        ReplaceEntries(entries);
    }

    public partial class TimingPointEntry : ListEntry
    {
        public readonly TimingPointInfo PointInfo;

        private SpriteText timeText;
        private SpriteText bpmText;
        private SpriteText signatureText;

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
            timeText = new SpriteText
            {
                Text = TimeUtils.Format(PointInfo.Time),
                Font = new FontUsage("Quicksand", 24, "Bold", false, true),
                Width = 100
            },
            bpmText = new SpriteText
            {
                Text = PointInfo.BPM + "bpm",
                Font = new FontUsage("Quicksand", 24, "Bold"),
                Width = 70
            },
            signatureText = new SpriteText
            {
                Text = PointInfo.Signature + "/4",
                Font = new FontUsage("Quicksand", 24, "Bold")
            }
        };

        public override PointSettings CreatePointSettings() => new TimingPointSettings(PointInfo);
    }
}
