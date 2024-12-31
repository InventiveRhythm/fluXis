using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class ScrollVelocityTag : EditorTag
{
    public override Colour4 TagColour => FluXisColors.ScrollVelocity;

    [Resolved]
    private PointsSidebar points { get; set; }

    public ScrollVelocity ScrollVelocity => (ScrollVelocity)TimedObject;

    public ScrollVelocityTag(EditorTagContainer parent, ScrollVelocity sv)
        : base(parent, sv)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{ScrollVelocity.Multiplier.ToStringInvariant("0.####")}x";
    }

    protected override bool OnClick(ClickEvent e)
    {
        points.ShowPoint(ScrollVelocity);
        return true;
    }
}
