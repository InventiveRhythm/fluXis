using fluXis.Game.Map;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TagTypes;

public partial class ScrollVelocityTag : EditorTag
{
    public override Colour4 TagColour => Colour4.FromHex("#00D4FF");

    public ScrollVelocityInfo ScrollVelocity => (ScrollVelocityInfo)TimedObject;

    public ScrollVelocityTag(EditorTagContainer parent, ScrollVelocityInfo sv)
        : base(parent, sv)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{ScrollVelocity.Multiplier}x";
    }
}
