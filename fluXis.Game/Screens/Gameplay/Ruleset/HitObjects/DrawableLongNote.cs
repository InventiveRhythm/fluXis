using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Gameplay.Ruleset.HitObjects.Long;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableLongNote : DrawableHitObject
{
    public override bool CanBeRemoved => Judged;

    public BindableBool IsBeingHeld { get; } = new();

    private Drawable bodyPiece;
    private DrawableLongNoteTail tailPiece;
    private DrawableLongNoteHead headPiece;

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
    }

    protected override void LoadComplete()
    {
        headPiece.OnJudgement += offset => OnHit?.Invoke(this, offset);
        headPiece.OnMiss += () => tailPiece.UpdateJudgement(false);
        tailPiece.OnJudgement += ApplyResult;

        IsBeingHeld.BindValueChanged(held =>
        {
            if (held.NewValue)
                headPiece.UpdateJudgement(true);
            else
                tailPiece.UpdateJudgement(true);
        });

        base.LoadComplete();
    }

    protected override void Update()
    {
        base.Update();

        if (IsBeingHeld.Value)
            Y = ObjectManager.HitPosition;

        var endY = ObjectManager.PositionAtTime(ScrollVelocityEndTime);
        var height = Y - endY;

        bodyPiece.Height = height;
        bodyPiece.Y = -headPiece.Height / 2;
        tailPiece.Y = -height;
    }

    public override void OnPressed(FluXisGameplayKeybind key)
    {
        if (key != Keybind || !ObjectManager.IsFirstInColumn(this))
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
