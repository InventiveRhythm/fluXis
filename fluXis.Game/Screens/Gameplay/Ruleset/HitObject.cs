using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class HitObject : CompositeDrawable
{
    public HitObjectInfo Data;
    public readonly float ScrollVelocityTime;
    public readonly float ScrollVelocityEndTime;

    private readonly HitObjectManager manager;

    private Drawable notePiece;
    private Drawable holdBodyPiece;
    private Drawable holdEndPiece;

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
    private void load(TextureStore textures)
    {
        Size = Receptor.SIZE;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;

        Colour4 color = FluXisColors.GetLaneColor(Data.Lane, manager.Map.KeyCount);

        InternalChildren = new[]
        {
            holdEndPiece = new Container
            {
                CornerRadius = 10,
                Masking = true,
                Height = 42,
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = color.Darken(.4f)
                },
                Alpha = 0
            },
            holdBodyPiece = new Container
            {
                RelativeSizeAxes = Axes.X,
                Width = 0.9f,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = ColourInfo.GradientVertical(color.Darken(.4f), color)
                },
                Alpha = 0
            },
            notePiece = new Container
            {
                CornerRadius = 10,
                Masking = true,
                Height = 42,
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = color
                }
            }
        };

        if (Data.IsLongNote())
        {
            holdBodyPiece.Alpha = 1;
            holdEndPiece.Alpha = 1;
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

        float hitY = receptor.Y - receptor.HitPosition;
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
