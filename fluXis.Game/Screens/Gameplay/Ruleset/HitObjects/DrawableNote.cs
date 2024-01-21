using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
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
    }

    protected override void CheckJudgement(bool byUser, float offset)
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
