using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Selection;

public partial class NoteSelectionBlueprint : SelectionBlueprint
{
    public new HitObjectInfo Object => (HitObjectInfo)base.Object;

    public new EditorHitObject Drawable => (EditorHitObject)base.Drawable;

    public override float SecondComparer => Object.HoldEndTime;

    public NoteSelectionBlueprint(TimedObject info)
        : base(info)
    {
        Width = EditorHitObjectContainer.NOTEWIDTH;
    }

    protected override void Update()
    {
        base.Update();

        if (Parent != null)
            Position = Parent.ToLocalSpace(HitObjectContainer.ScreenSpacePositionAtTime(Object.Time, Object.Lane));
    }
}
