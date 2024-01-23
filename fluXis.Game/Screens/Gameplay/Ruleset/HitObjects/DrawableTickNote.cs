using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
using fluXis.Game.Skinning.Default.HitObject;
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
        InternalChild = new DefaultTickNote();
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
