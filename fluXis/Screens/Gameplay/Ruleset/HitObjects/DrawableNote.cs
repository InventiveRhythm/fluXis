using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Skinning.Bases.HitObjects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableNote : DrawableHitObject
{
    public override bool CanBeRemoved => Judged || Time.Current - Data.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    public DrawableNote(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = Skin.GetHitObject(VisualLane, ObjectManager.KeyCount).With(d =>
        {
            d.Anchor = Anchor.BottomCentre;
            d.Origin = Anchor.BottomCentre;
        });

        if (ObjectManager.UseSnapColors)
        {
            var child = InternalChild as ICanHaveSnapColor;
            child?.ApplySnapColor(Column.GetSnapIndex(Data.Time), 0);
        }
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
            return;
        }

        if (!HitWindows.CanBeHit(offset))
            return;

        ApplyResult(offset);
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind || !Column.IsFirst(this))
            return;

        UpdateJudgement(true);
    }
}
