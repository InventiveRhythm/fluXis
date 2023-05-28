using fluXis.Game.Audio;
using fluXis.Game.Map;
using fluXis.Game.Skinning;
using fluXis.Game.Skinning.Default.HitObject;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class HitObject : CompositeDrawable
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    public HitObjectInfo Data;
    public readonly float ScrollVelocityTime;
    public readonly float ScrollVelocityEndTime;

    private readonly HitObjectManager manager;

    private DefaultHitObjectPiece notePiece;
    private DefaultHitObjectBody holdBodyPiece;
    private DefaultHitObjectEnd holdEndPiece;

    public bool Hitable;
    public bool Releasable;
    public bool GotHit;
    public bool Missed;
    public bool IsBeingHeld;
    public bool LongNoteMissed;
    public bool Exists = true;

    public HitObject(HitObjectManager manager, HitObjectInfo data)
    {
        this.manager = manager;
        Data = data;
        ScrollVelocityTime = manager.PositionFromTime(data.Time);
        ScrollVelocityEndTime = manager.PositionFromTime(data.HoldEndTime);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;

        InternalChildren = new Drawable[]
        {
            holdEndPiece = new DefaultHitObjectEnd(),
            holdBodyPiece = new DefaultHitObjectBody(),
            notePiece = new DefaultHitObjectPiece()
        };

        notePiece.UpdateColor(Data.Lane, manager.Map.KeyCount);
        holdBodyPiece.UpdateColor(Data.Lane, manager.Map.KeyCount);
        holdEndPiece.UpdateColor(Data.Lane, manager.Map.KeyCount);

        if (!Data.IsLongNote())
        {
            holdBodyPiece.Alpha = 0;
            holdEndPiece.Alpha = 0;
        }
    }

    public void Kill()
    {
        Alpha = 0;
        Exists = false;
    }

    public void MissLongNote()
    {
        LongNoteMissed = true;
        this.FadeTo(.5f, 100).FadeColour(Colour4.Red, 100);
    }

    public bool IsOffScreen()
    {
        if (Data.IsLongNote())
            return holdEndPiece.Y - holdEndPiece.Height > 0;

        return notePiece.Y - notePiece.Height > manager.Playfield.Receptors[0].Y;
    }

    protected override void Update()
    {
        Missed = (Conductor.CurrentTime - Data.Time > 150 && !IsBeingHeld) || (Data.IsLongNote() && IsBeingHeld && Conductor.CurrentTime - Data.HoldEndTime > 150);
        Hitable = Conductor.CurrentTime - Data.Time > -150 && !Missed;
        Releasable = Data.IsLongNote() && Conductor.CurrentTime - Data.HoldEndTime > -150 && !Missed;

        if (!Exists) return;

        var receptor = manager.Playfield.Receptors[Data.Lane - 1];

        X = receptor.X;
        Width = receptor.Width;

        float hitY = receptor.Y - skinManager.CurrentSkin.HitPosition;
        notePiece.Y = hitY - .5f * ((ScrollVelocityTime - manager.CurrentTime) * manager.ScrollSpeed);

        if (IsBeingHeld) notePiece.Y = hitY;

        if (Data.IsLongNote())
        {
            var endY = hitY - .5f * ((ScrollVelocityEndTime - manager.CurrentTime) * manager.ScrollSpeed);

            var height = notePiece.Y - endY;
            holdBodyPiece.Size = new Vector2(holdBodyPiece.Width, height);
            holdBodyPiece.Y = notePiece.Y - notePiece.Height / 2;

            holdEndPiece.Y = endY;
        }

        IsBeingHeld = false;

        base.Update();
    }
}
