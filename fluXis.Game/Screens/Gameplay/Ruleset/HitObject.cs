using fluXis.Game.Audio;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset
{
    public class HitObject : CompositeDrawable
    {
        public HitObjectInfo Data;
        public readonly float ScrollVelocityTime;
        public readonly float ScrollVelocityEndTime;

        private readonly HitObjectManager manager;
        private Sprite spr;
        private Sprite holdBodySpr;
        private Sprite holdEndSpr;

        public bool Hitable;
        public bool Releasable;
        public bool GotHit;
        public bool Missed;
        public bool IsBeingHeld;
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
            X = manager.Playfield.Receptors[Data.Lane - 1].X;

            InternalChildren = new Drawable[]
            {
                holdEndSpr = new Sprite
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Texture = textures.Get("gameplay/hitobject/holdend-" + Data.Lane),
                    Alpha = 0
                },
                holdBodySpr = new Sprite
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Texture = textures.Get("gameplay/hitobject/holdbody-" + Data.Lane),
                    Alpha = 0
                },
                spr = new Sprite
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    RelativeSizeAxes = Axes.Both,
                    Texture = textures.Get("gameplay/hitobject/hitobject-" + Data.Lane),
                },
            };

            if (Data.IsLongNote())
            {
                holdBodySpr.Alpha = 1;
                holdEndSpr.Alpha = 1;

                var factor = Receptor.SIZE.X / holdBodySpr.Size.X;
                holdEndSpr.Scale = new Vector2(factor, factor);
            }
        }

        public void Kill(bool missed)
        {
            if (missed)
                this.FadeOut(100).FadeColour(Colour4.Red, 100).ScaleTo(0.9f, 100);
            else
                Alpha = 0;

            Exists = false;
        }

        protected override void Update()
        {
            Missed = (Conductor.CurrentTime - Data.Time > 150 && !IsBeingHeld) || (Data.IsLongNote() && IsBeingHeld && Conductor.CurrentTime - Data.HoldEndTime > 150);
            Hitable = Conductor.CurrentTime - Data.Time > -150 && !Missed;
            Releasable = Data.IsLongNote() && Conductor.CurrentTime - Data.HoldEndTime > -150 && !Missed;

            if (!Exists)
                return;

            var receptor = manager.Playfield.Receptors[Data.Lane - 1];
            spr.Y = receptor.Y - .5f * ((ScrollVelocityTime - manager.CurrentTime) * manager.ScrollSpeed);

            if (IsBeingHeld)
                spr.Y = receptor.Y;

            if (Data.IsLongNote())
            {
                var endY = receptor.Y - .5f * ((ScrollVelocityEndTime - manager.CurrentTime) * manager.ScrollSpeed);

                var height = endY - spr.Y;
                holdBodySpr.Size = new Vector2(Receptor.SIZE.X, height);
                holdBodySpr.Y = spr.Y + Receptor.SIZE.Y / 2;

                holdEndSpr.Y = endY;
            }

            IsBeingHeld = false;

            base.Update();
        }
    }
}
