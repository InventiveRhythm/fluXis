using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
using fluXis.Game.Skinning.Bases.HitObjects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;

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
        InternalChild = SkinManager.GetHitObject(Data.Lane, ObjectManager.KeyCount).With(d =>
        {
            d.Anchor = Anchor.BottomCentre;
            d.Origin = Anchor.BottomCentre;
        });

        if (ObjectManager.UseSnapColors)
        {
            var child = InternalChild as ICanHaveSnapColor;
            child?.ApplySnapColor(ObjectManager.GetSnapIndex(Data.Time), 0);
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
        if (key != Keybind || !ObjectManager.IsFirstInColumn(this))
            return;

        UpdateJudgement(true);
    }
}
