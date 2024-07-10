using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class ScrollVelocityTag : EditorTag
{
    public override Colour4 TagColour => FluXisColors.ScrollVelocity;

    public ScrollVelocity ScrollVelocity => (ScrollVelocity)TimedObject;

    public ScrollVelocityTag(EditorTagContainer parent, ScrollVelocity sv)
        : base(parent, sv)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{ScrollVelocity.Multiplier}x";
    }
}
