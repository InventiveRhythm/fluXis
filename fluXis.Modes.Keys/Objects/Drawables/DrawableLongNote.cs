using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Modes.Gameplay.Objects;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Modes.Keys.Objects.Drawables;

public partial class DrawableLongNote : DrawableKeysHitObject<LongNote>, IKeyBindingHandler<FluXisGameplayKeybind>
{
    public override bool CanBeRemoved => Judged && tail.CanBeRemoved;

    private Drawable body = null!;
    private DrawableLongNoteTail tail = null!;

    public DrawableLongNote(LongNote o)
        : base(o)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        tail.Anchor = Anchor.BottomLeft;

        AddRangeInternal([
            body = Skin.GetLongNoteBody(PlayerLane, Manager.KeyCount).WithRelativeSize(Axes.X).WithAnchor(Anchor.BottomCentre),
            Skin.GetLongNoteStart(PlayerLane, Manager.KeyCount).WithRelativeSize(Axes.X).WithAnchor(Anchor.BottomLeft),
            tail.CreateProxy()
        ]);
    }

    protected override DrawableHitObject? CreateNested(HitObject obj)
    {
        switch (obj)
        {
            case LongNoteTail t:
                return tail = new DrawableLongNoteTail(t, this);
        }

        return base.CreateNested(obj);
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            ApplyResult(HitWindows.Lowest);
            return;
        }

        if (!HitWindows.CanBeHit(offset))
            return;

        var result = HitWindows.JudgementFor(offset);
        ApplyResult(result);
    }

    protected override void Update()
    {
        base.Update();

        if (!Judged && TimeDelta < -HitWindows.TimingFor(HitWindows.Lowest))
            UpdateJudgement(false);
    }

    protected override void UpdatePosition()
    {
        base.UpdatePosition();

        if (Judged && !tail.Judged)
            Y = Manager.HitPosition;

        var tailY = Manager.PositionAtTime(tail.ScrollVelocityTime, tail.Object.ScrollGroup);
        var height = Y - tailY;

        tail.Y = -height;
        body.Height = height;
        body.Y = -tail.DrawHeight / 2f;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e)
        => e.Action == Action && UpdateJudgement(true);

    public void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) { }
}
