using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning;
using fluXis.Game.Skinning.Default.HitObject;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class HitObject : CompositeDrawable
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    public HitObjectInfo Data { get; }
    private double scrollVelocityTime { get; }
    private double scrollVelocityEndTime { get; }

    private readonly HitObjectManager manager;

    private Drawable notePiece;
    private Drawable holdBodyPiece;
    private Drawable holdEndPiece;

    public bool Hitable { get; private set; }
    public bool Releasable { get; private set; }
    public bool GotHit { get; set; }
    public bool Missed { get; private set; }
    public bool IsBeingHeld { get; set; }
    public bool LongNoteMissed { get; private set; }

    public HitObject(HitObjectManager manager, HitObjectInfo data)
    {
        this.manager = manager;
        Data = data;
        scrollVelocityTime = manager.ScrollVelocityPositionFromTime(data.Time);
        scrollVelocityEndTime = manager.ScrollVelocityPositionFromTime(data.HoldEndTime);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;

        InternalChildren = new[]
        {
            holdBodyPiece = skinManager.GetLongNoteBody(Data.Lane, manager.KeyCount).With(d => d.Alpha = Data.IsLongNote() ? 1 : 0),
            holdEndPiece = skinManager.GetLongNoteEnd(Data.Lane, manager.KeyCount).With(d => d.Alpha = Data.IsLongNote() ? 1 : 0),
            notePiece = skinManager.GetHitObject(Data.Lane, manager.KeyCount)
        };

        if (manager.UseSnapColors)
        {
            var colorStart = FluXisColors.GetSnapColor(manager.GetSnapIndex((int)Data.Time));
            var colorEnd = FluXisColors.GetSnapColor(manager.GetSnapIndex((int)Data.HoldEndTime));

            if (notePiece is DefaultHitObjectPiece defaultPiece) defaultPiece.SetColor(colorStart);
            else notePiece.Colour = colorStart;

            if (holdBodyPiece is DefaultHitObjectBody defaultBody) defaultBody.SetColor(colorStart, colorEnd.Darken(.4f));
            else holdBodyPiece.Colour = ColourInfo.GradientVertical(colorEnd.Darken(.4f), colorStart);

            if (holdEndPiece is DefaultHitObjectEnd defaultEnd) defaultEnd.SetColor(colorEnd.Darken(.4f));
            else holdEndPiece.Colour = colorEnd.Darken(.4f);
        }
    }

    public void MissLongNote()
    {
        LongNoteMissed = true;
        this.FadeColour(Colour4.Red, 100);
    }

    protected override void Update()
    {
        base.Update();

        updateTiming();
        updatePositioning();

        // reset for next frame
        IsBeingHeld = false;
    }

    private void updateTiming()
    {
        var lastHitableTime = screen.HitWindows.TimingFor(screen.HitWindows.LowestHitable);
        var missTime = screen.HitWindows.TimingFor(Judgement.Miss);
        var releaseMissTime = screen.ReleaseWindows.TimingFor(screen.ReleaseWindows.Lowest);

        Missed = (Clock.CurrentTime - Data.Time > lastHitableTime && !IsBeingHeld) || (Data.IsLongNote() && IsBeingHeld && Clock.CurrentTime - Data.HoldEndTime > releaseMissTime);
        Hitable = Clock.CurrentTime - Data.Time > -missTime && !Missed;
        Releasable = Data.IsLongNote() && Clock.CurrentTime - Data.HoldEndTime > -releaseMissTime && !Missed;
    }

    private void updatePositioning()
    {
        X = manager.PositionAtLane(Data.Lane);
        Y = manager.PositionAtTime(scrollVelocityTime);
        Width = manager.WidthOfLane(Data.Lane);

        if (IsBeingHeld)
            Y = manager.HitPosition;

        if (!Data.IsLongNote()) return;

        var endY = manager.PositionAtTime(scrollVelocityEndTime);
        var diff = Y - endY;

        holdBodyPiece.Height = diff;
        holdBodyPiece.Y = -holdEndPiece.Height / 2;
        holdEndPiece.Y = -diff;
    }
}
