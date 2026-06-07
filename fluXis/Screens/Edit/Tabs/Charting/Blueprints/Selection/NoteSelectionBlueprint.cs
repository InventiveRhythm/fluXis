using System;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class NoteSelectionBlueprint : ChartingSelectionBlueprint
{
    public EditorHitObject HitObject => Drawable as EditorHitObject;

    public override double FirstComparer => Object.Time;
    public override double SecondComparer => Object.EndTime;

    public override Vector2 ScreenSpaceSelectionPoint => PositionProvider.ScreenSpacePositionAtTime(Object.Time, Object.Lane);
    public override bool Visible => Math.Abs(EditorClock.CurrentTime - Object.Time) <= 4000;

    protected ITimePositionProvider PositionProvider
    {
        get
        {
            var index = (Object.Lane - 1) / Map.RealmMap.KeyCount;
            return ChartingContainer.Playfields[index];
        }
    }

    public NoteSelectionBlueprint(HitObject info)
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
