using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.List;

public partial class ScrollVelocityList : TimingCategoryList<ScrollVelocityList.ScrollVelocityEntry>
{
    public ScrollVelocityList(List<ScrollVelocity> velocities, TimingTab tab)
        : base("Scroll Velocities", FluXisColors.Background2, tab)
    {
        velocities.ForEach(point => AddEntry(new ScrollVelocityEntry(point)));
    }

    public override void OnAdd()
    {
        var point = new ScrollVelocity
        {
            Time = (float)EditorClock.CurrentTime,
            Multiplier = 1.0f
        };

        AddEntry(new ScrollVelocityEntry(point));
        EditorValues.MapInfo.Add(point);

        base.OnAdd();
    }

    public override void Sort()
    {
        var entries = GetEntries().ToList();

        entries.Sort((a, b) => a.ScrollVelocity.Time.CompareTo(b.ScrollVelocity.Time));
        ReplaceEntries(entries);
    }

    public override void Remove(ScrollVelocityEntry entry)
    {
        EditorValues.MapInfo.Remove(entry.ScrollVelocity);
        base.Remove(entry);
    }

    public partial class ScrollVelocityEntry : ListEntry
    {
        public readonly ScrollVelocity ScrollVelocity;

        private FluXisSpriteText timeText;
        private FluXisSpriteText multiplierText;

        public ScrollVelocityEntry(ScrollVelocity scrollVelocity)
        {
            ScrollVelocity = scrollVelocity;
        }

        protected override void Update()
        {
            timeText.Text = TimeUtils.Format(ScrollVelocity.Time);
            multiplierText.Text = ScrollVelocity.Multiplier + "x";
            base.Update();
        }

        public override Drawable[] CreateContent() => new Drawable[]
        {
            timeText = new FluXisSpriteText
            {
                Text = TimeUtils.Format(ScrollVelocity.Time),
                FontSize = 24,
                Width = 100
            },
            multiplierText = new FluXisSpriteText
            {
                Text = ScrollVelocity.Multiplier + "x",
                FontSize = 24
            }
        };

        public override PointSettings CreatePointSettings() => new ScrollVelocitySettings(ScrollVelocity);

        protected override void Remove() => List.Remove(this);
    }
}
