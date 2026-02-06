using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Utils;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class ScrollVelocityTag : EditorTag
{
    public override Colour4 TagColour => Theme.ScrollVelocity;

    private ScrollVelocity velocity => (ScrollVelocity)TimedObject;

    public ScrollVelocityTag(EditorTagContainer parent, ScrollVelocity sv)
        : base(parent, sv)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{velocity.Multiplier.ToStringInvariant("0.####")}x";
    }

    protected override bool OnClick(ClickEvent e)
    {
        Editor.ChangeToTab<DesignTab>(x => x.Container.Sidebar.ShowPoint(velocity));
        return true;
    }
}
