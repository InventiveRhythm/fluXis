using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class TimingPointTag : EditorTag
{
    public override Colour4 TagColour => Theme.TimingPoint;

    [Resolved]
    private EditorTagDependencies deps { get; set; }

    public TimingPoint TimingPoint => (TimingPoint)TimedObject;

    public TimingPointTag(EditorTagContainer parent, TimingPoint tp)
        : base(parent, tp)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{(int)TimingPoint.BPM}BPM";
    }

    protected override bool OnClick(ClickEvent e)
    {
        deps.ShowPointInCharting(TimingPoint);
        return true;
    }
}
