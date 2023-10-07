using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning;
using fluXis.Game.Skinning.Default.HitObject;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class HitObject : CompositeDrawable
{
    [Resolved]
    private SkinManager skinManager { get; set; }

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
        scrollVelocityTime = manager.PositionFromTime(data.Time);
        scrollVelocityEndTime = manager.PositionFromTime(data.HoldEndTime);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;

        InternalChildren = new[]
        {
            holdBodyPiece = skinManager.GetLongNoteBody(Data.Lane, manager.Map.KeyCount),
            holdEndPiece = skinManager.GetLongNoteEnd(Data.Lane, manager.Map.KeyCount),
            notePiece = skinManager.GetHitObject(Data.Lane, manager.Map.KeyCount)
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

        if (!Data.IsLongNote())
        {
            holdBodyPiece.Alpha = 0;
            holdEndPiece.Alpha = 0;
        }
    }

    public void MissLongNote()
    {
        LongNoteMissed = true;
        this.FadeTo(.5f, 100).FadeColour(Colour4.Red, 100);
    }

    public bool IsOffScreen()
    {
        return Data.IsLongNote()
            ? holdEndPiece.Y - holdEndPiece.Height > 0
            : notePiece.Y - notePiece.Height > manager.Playfield.Receptors[0].Y;
    }

    protected override void Update()
    {
        var lastHitableTime = manager.Playfield.Screen.HitWindows.TimingFor(manager.Playfield.Screen.HitWindows.LowestHitable);
        var missTime = manager.Playfield.Screen.HitWindows.TimingFor(Judgement.Miss);
        var releaseMissTime = manager.Playfield.Screen.ReleaseWindows.TimingFor(manager.Playfield.Screen.ReleaseWindows.Lowest);

        Missed = (Clock.CurrentTime - Data.Time > lastHitableTime && !IsBeingHeld) || (Data.IsLongNote() && IsBeingHeld && Clock.CurrentTime - Data.HoldEndTime > releaseMissTime);
        Hitable = Clock.CurrentTime - Data.Time > -missTime && !Missed;
        Releasable = Data.IsLongNote() && Clock.CurrentTime - Data.HoldEndTime > -releaseMissTime && !Missed;

        var receptor = manager.Playfield.Receptors[Data.Lane - 1];

        X = receptor.X;
        Width = receptor.Width;

        var scrollSpeed = manager.ScrollSpeed;

        float hitY = receptor.Y - skinManager.CurrentSkin.GetKeymode(manager.Map.KeyCount).HitPosition;
        notePiece.Y = (float)(hitY - .5f * ((scrollVelocityTime - manager.CurrentTime) * scrollSpeed));

        if (IsBeingHeld) notePiece.Y = hitY;

        if (Data.IsLongNote())
        {
            var endY = hitY - .5f * ((scrollVelocityEndTime - manager.CurrentTime) * scrollSpeed);

            var height = notePiece.Y - endY;
            holdBodyPiece.Size = new Vector2(holdBodyPiece.Width, (float)height);
            holdBodyPiece.Y = notePiece.Y - notePiece.Height / 2;

            holdEndPiece.Y = (float)endY;
        }

        IsBeingHeld = false;

        base.Update();
    }
}
