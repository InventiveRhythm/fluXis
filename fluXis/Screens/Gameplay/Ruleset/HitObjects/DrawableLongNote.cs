using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Scoring.Enums;
using fluXis.Screens.Gameplay.Ruleset.HitObjects.Long;
using fluXis.Skinning.Bases.HitObjects;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableLongNote : DrawableHitObject
{
    public override bool CanBeRemoved
    {
        get
        {
            if (missed)
                return Time.Current > Data.EndTime;

            return Judged;
        }
    }

    public BindableBool IsBeingHeld { get; } = new();

    private Drawable bodyPiece;
    private DrawableLongNoteTail tailPiece;
    private DrawableLongNoteHead headPiece;

    private bool missed;

    private readonly Colour4 missTint = new(.4f, .4f, .4f, 1);

    public DrawableLongNote(HitObject data)
        : base(data)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new[]
        {
            bodyPiece = SkinManager.GetLongNoteBody(VisualLane, ObjectManager.KeyCount).With(d =>
            {
                d.Anchor = Anchor.BottomCentre;
                d.Origin = Anchor.BottomCentre;
            }),
            tailPiece = new DrawableLongNoteTail(Data),
            headPiece = new DrawableLongNoteHead(Data)
        };

        if (ObjectManager.UseSnapColors)
        {
            var startIdx = Column.GetSnapIndex(Data.Time);
            var endIdx = Column.GetSnapIndex(Data.EndTime);

            if (bodyPiece is ICanHaveSnapColor colorable)
                colorable.ApplySnapColor(startIdx, endIdx);

            tailPiece.ApplySnapColor(endIdx);
            headPiece.ApplySnapColor(startIdx);
        }
    }

    protected override void LoadComplete()
    {
        headPiece.OnJudgement += offset => OnHit?.Invoke(this, offset);
        headPiece.OnMiss += () => tailPiece.UpdateJudgement(false);
        tailPiece.OnJudgement += diff =>
        {
            var judgement = Ruleset.ReleaseWindows.JudgementFor(diff);

            if (judgement == Judgement.Alright)
                miss();

            ApplyResult(diff);
        };

        IsBeingHeld.BindValueChanged(held =>
        {
            if (held.NewValue)
                headPiece.UpdateJudgement(true);
            else
                tailPiece.UpdateJudgement(true);
        });

        base.LoadComplete();
    }

    private void miss()
    {
        missed = true;
        this.FadeColour(missTint, 100);
    }

    protected override void Update()
    {
        base.Update();

        if (IsBeingHeld.Value)
            Y = ObjectManager.HitPosition;

        var endY = Column.PositionAtTime(ScrollVelocityEndTime, Data.EndEasing);
        var height = Y - endY;

        bodyPiece.Height = height;
        bodyPiece.Y = -headPiece.Height / 2;
        tailPiece.Y = -height;
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind || !Column.IsFirst(this))
            return;

        if (!headPiece.Hittable || missed)
            return;

        IsBeingHeld.Value = true;
    }

    public override void OnReleased(FluXisGameplayKeybind key)
    {
        if (key != Keybind)
            return;

        IsBeingHeld.Value = false;
    }
}
