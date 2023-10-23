using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TagTypes;

public partial class PreviewPointTag : EditorTag
{
    [Resolved]
    private EditorValues values { get; set; }

    public override Colour4 TagColour => Colour4.FromHex("FDD27F");

    public PreviewPointTag(EditorTagContainer parent)
        : base(parent, new TimedObject())
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Text.Text = "Preview";
    }

    protected override void Update()
    {
        TimedObject.Time = values.MapInfo.Metadata.PreviewTime;
        base.Update();
    }
}
