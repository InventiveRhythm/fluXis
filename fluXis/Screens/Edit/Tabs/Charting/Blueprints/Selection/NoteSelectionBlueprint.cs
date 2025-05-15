using System;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class NoteSelectionBlueprint : ChartingSelectionBlueprint
{
    public new HitObject Object => (HitObject)base.Object;

    public EditorHitObject HitObject => Drawable as EditorHitObject;

    public override double FirstComparer => Object.Time;
    public override double SecondComparer => Object.EndTime;

    public override bool Visible => Math.Abs(EditorClock.CurrentTime - Object.Time) <= 4000;

    protected ITimePositionProvider PositionProvider
    {
        get
        {
            var index = (Object.Lane - 1) / Map.RealmMap.KeyCount;
            return ChartingContainer.Playfields[index];
        }
    }

    public NoteSelectionBlueprint(ITimedObject info)
        : base(info)
    {
        Width = EditorHitObjectContainer.NOTEWIDTH;
    }

    public override void UpdatePosition(Drawable parent)
    {
        base.UpdatePosition(parent);

        if (parent != null)
            Position = parent.ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(Object.Time, Object.Lane));
    }
}
