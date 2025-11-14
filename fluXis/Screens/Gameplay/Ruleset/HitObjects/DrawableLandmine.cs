using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Scoring.Enums;
using fluXis.Screens.Gameplay.Input;
using osu.Framework.Allocation;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableLandmine : DrawableHitObject
{
    //public override HitWindows HitWindows => Ruleset.LandmineWindows;

    public override bool CanBeRemoved => Judged || didNotGetHit;

    private bool didNotGetHit
    {
        get
        {
            //if the landmine is in the timing window of the next regular or long note, judge it as soon as it passes the receptors
            if (nextNote != null && TimeDelta <= 0)
                return true;

            return TimeDelta <= -Ruleset.LandmineWindows.TimingFor(Judgement.Miss);
        }
    }

    [Resolved]
    private GameplayInput input { get; set; }

    private bool isBeingHeld;

    //Next non-landmine HitObject on the column. Only set if the landmine is in the hit window of the next note, null otherwise.
    private HitObject nextNote;

    public DrawableLandmine(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        //Masking = false; //needed for using the cross in bar skin
        InternalChildren = new[]
        {
            Skin.GetLandmine(VisualLane, ObjectManager.KeyCount)
        };

        AlwaysPresent = true;
        if (Data.Hidden) Alpha = 0;
        //if (Data.Hidden) InternalChild.Alpha = 0;

        nextNote = findNextNote();
    }

    protected override void Update()
    {
        base.Update();

        //TODO (?): make it more forgiving if the landmine is too close of the previous note?
        if (isBeingHeld)
            UpdateJudgement(true);
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            if (isBeingHeld)
            {
                ApplyResult(0);
                return;
            }

            //don't give any judgement the landmine if it isn't triggered by the user
            //TODO (?) : dedicated "Ignored" judgement, or use "None" judgement?
            return;
        }

        if (isBeingHeld)
        {
            if (offset < 0)
                ApplyResult(0);
            return;
        }

        if (Ruleset.LandmineWindows.CanBeHit(offset))
            ApplyResult(offset);
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        if (Column.IsFirst(this))
            UpdateJudgement(true);

        isBeingHeld = true;
    }

    public override void OnReleased(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        isBeingHeld = false;
    }

    private HitObject findNextNote()
    {
        HitObject next = Data.NextObject;

        while (next != null)
        {
            if (next.Time - Data.Time > Ruleset.HitWindows.TimingFor(Judgement.Okay)) return null; //no need to keep going if the next note is too far away
            if (next.Type != 2) return next;

            next = next.NextObject;
        }

        return null;
    }
}
