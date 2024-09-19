using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Gameplay.Ruleset.HitObjects.Long;
using fluXis.Game.Skinning.Bases.HitObjects;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;

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

    private Easing endEasing = Easing.None;

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
            bodyPiece = SkinManager.GetLongNoteBody(Data.Lane, ObjectManager.KeyCount).With(d =>
            {
                d.Anchor = Anchor.BottomCentre;
                d.Origin = Anchor.BottomCentre;
            }),
            tailPiece = new DrawableLongNoteTail(Data),
            headPiece = new DrawableLongNoteHead(Data)
        };

        endEasing = ObjectManager.EasingAtTime(Data.EndTime);

        if (ObjectManager.UseSnapColors)
        {
            var startIdx = ObjectManager.GetSnapIndex(Data.Time);
            var endIdx = ObjectManager.GetSnapIndex(Data.EndTime);

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
            var judgement = Screen.ReleaseWindows.JudgementFor(diff);

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

        var endY = ObjectManager.PositionAtTime(ScrollVelocityEndTime, endEasing);
        var height = Y - endY;

        bodyPiece.Height = height;
        bodyPiece.Y = -headPiece.Height / 2;
        tailPiece.Y = -height;
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind || !ObjectManager.IsFirstInColumn(this))
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
