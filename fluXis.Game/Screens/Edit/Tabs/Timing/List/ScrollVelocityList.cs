using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.List;

public partial class ScrollVelocityList : TimingCategoryList<ScrollVelocityList.ScrollVelocityEntry>
{
    public ScrollVelocityList(List<ScrollVelocityInfo> velocities)
        : base("Scroll Velocities", FluXisColors.Background2)
    {
        velocities.ForEach(point => AddEntry(new ScrollVelocityEntry(point)));
    }

    public override void OnAdd()
    {
        AddEntry(new ScrollVelocityEntry(new ScrollVelocityInfo
        {
            Time = Conductor.CurrentTime,
            Multiplier = 1.0f
        }));

        base.OnAdd();
    }

    public override void Sort()
    {
        var entries = GetEntries().ToList();

        entries.Sort((a, b) => a.VelocityInfo.Time.CompareTo(b.VelocityInfo.Time));
        ReplaceEntries(entries);
    }

    public partial class ScrollVelocityEntry : ListEntry
    {
        public readonly ScrollVelocityInfo VelocityInfo;

        public ScrollVelocityEntry(ScrollVelocityInfo velocityInfo)
        {
            VelocityInfo = velocityInfo;
        }

        public override Drawable[] CreateContent() => new Drawable[]
        {
            new SpriteText
            {
                Text = TimeUtils.Format(VelocityInfo.Time),
                Font = new FontUsage("Quicksand", 24, "Bold", false, true),
                Width = 100
            },
            new SpriteText
            {
                Text = VelocityInfo.Multiplier + "x",
                Font = new FontUsage("Quicksand", 24, "Bold")
            }
        };
    }
}
