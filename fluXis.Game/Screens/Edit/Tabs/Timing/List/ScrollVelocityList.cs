using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.List;

public partial class ScrollVelocityList : TimingCategoryList<ScrollVelocityList.ScrollVelocityEntry>
{
    public ScrollVelocityList(List<ScrollVelocityInfo> velocities, TimingTab tab)
        : base("Scroll Velocities", FluXisColors.Background2, tab)
    {
        velocities.ForEach(point => AddEntry(new ScrollVelocityEntry(point)));
    }

    public override void OnAdd()
    {
        var point = new ScrollVelocityInfo
        {
            Time = (float)EditorClock.CurrentTime,
            Multiplier = 1.0f
        };

        AddEntry(new ScrollVelocityEntry(point));
        EditorValues.MapInfo.ScrollVelocities.Add(point);

        base.OnAdd();
    }

    public override void Sort()
    {
        var entries = GetEntries().ToList();

        entries.Sort((a, b) => a.VelocityInfo.Time.CompareTo(b.VelocityInfo.Time));
        ReplaceEntries(entries);
    }

    public override void Remove(ScrollVelocityEntry entry)
    {
        EditorValues.MapInfo.ScrollVelocities.Remove(entry.VelocityInfo);
        base.Remove(entry);
    }

    public partial class ScrollVelocityEntry : ListEntry
    {
        public readonly ScrollVelocityInfo VelocityInfo;

        private FluXisSpriteText timeText;
        private FluXisSpriteText multiplierText;

        public ScrollVelocityEntry(ScrollVelocityInfo velocityInfo)
        {
            VelocityInfo = velocityInfo;
        }

        protected override void Update()
        {
            timeText.Text = TimeUtils.Format(VelocityInfo.Time);
            multiplierText.Text = VelocityInfo.Multiplier + "x";
            base.Update();
        }

        public override Drawable[] CreateContent() => new Drawable[]
        {
            timeText = new FluXisSpriteText
            {
                Text = TimeUtils.Format(VelocityInfo.Time),
                FontSize = 24,
                Width = 100
            },
            multiplierText = new FluXisSpriteText
            {
                Text = VelocityInfo.Multiplier + "x",
                FontSize = 24
            }
        };

        public override PointSettings CreatePointSettings() => new ScrollVelocitySettings(VelocityInfo);

        protected override void Remove() => List.Remove(this);
    }
}
