using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableTickNote : DrawableHitObject
{
    public override bool CanBeRemoved => Judged || Time.Current - Data.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    private bool isBeingHeld;

    public DrawableTickNote(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = SkinManager.GetTickNote(Data.Lane, ObjectManager.KeyCount);
    }

    protected override void Update()
    {
        base.Update();

        if (isBeingHeld)
            UpdateJudgement(true);
    }

    protected override void CheckJudgement(bool byUser, float offset)
    {
        if (!byUser)
        {
            ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
            return;
        }

        if (offset >= 0)
            return;

        ObjectManager.PlayHitSound(Data, false);
        ApplyResult(offset);
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        isBeingHeld = true;
    }

    public override void OnReleased(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        isBeingHeld = false;
    }
}
