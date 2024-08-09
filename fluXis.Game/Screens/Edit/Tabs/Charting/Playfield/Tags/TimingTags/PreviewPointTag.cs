using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class PreviewPointTag : EditorTag
{
    [Resolved]
    private EditorMap map { get; set; }

    public override Colour4 TagColour => FluXisColors.PreviewPoint;

    public PreviewPointTag(EditorTagContainer parent)
        : base(parent, new PreviewPointObject())
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Text.Text = "Preview";
    }

    protected override void Update()
    {
        TimedObject.Time = map.MapInfo.Metadata.PreviewTime;
        base.Update();
    }

    // placeholder class for the preview point
    private class PreviewPointObject : ITimedObject
    {
        public double Time { get; set; }
    }
}
