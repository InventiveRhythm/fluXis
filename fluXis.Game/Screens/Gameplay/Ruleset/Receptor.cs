using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset
{
    public class Receptor : CompositeDrawable
    {
        public static readonly Vector2 SIZE = new Vector2(114, 114);
        public Playfield Playfield;

        private readonly int id;
        private Sprite sprUp;
        private Sprite sprDown;

        public Receptor(Playfield playfield, int id)
        {
            Playfield = playfield;
            this.id = id;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Origin = Anchor.BottomCentre;
            Anchor = Anchor.BottomCentre;
            Size = SIZE;
            X = (id - 1.5f) * SIZE.X;
            Y = -60;

            InternalChildren = new Drawable[]
            {
                sprUp = new Sprite
                {
                    Name = "Sprite Up",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = SIZE,
                    Texture = textures.Get("gameplay/receptor/up-" + (id + 1))
                },
                sprDown = new Sprite
                {
                    Name = "Sprite Down",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = SIZE,
                    Alpha = 0,
                    Texture = textures.Get("gameplay/receptor/down-" + (id + 1))
                }
            };
        }

        protected override void LoadComplete()
        {
            const float duration = 500f;
            const float delay = 80f;

            this.RotateTo(7)
                .MoveToY(Y + 100)
                .FadeOut()
                .Then(delay * id)
                .RotateTo(0, duration, Easing.OutCubic)
                .MoveToY(Y - 100, duration, Easing.OutCubic)
                .FadeInFromZero(duration);

            base.LoadComplete();
        }

        public void OnExit()
        {
            const float duration = 500f;
            const float delay = 80f;

            this.RotateTo(0)
                .Then(delay * id)
                .RotateTo(-7, duration, Easing.InCubic)
                .MoveToY(Y - 100, duration, Easing.InCubic)
                .FadeOutFromOne(duration);
        }

        protected override void Update()
        {
            if (Playfield.Screen.Input.Pressed[id])
            {
                sprDown.Alpha = 1;
                sprUp.Alpha = 0;
            }
            else
            {
                sprDown.Alpha = 0;
                sprUp.Alpha = 1;
            }

            base.Update();
        }
    }
}
