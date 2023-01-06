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
            X = (id - (Playfield.Manager.Map.KeyCount / 2f - .5f)) * SIZE.X;
            Y = -60;

            InternalChildren = new Drawable[]
            {
                sprUp = new Sprite
                {
                    Name = "Sprite Up",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = SIZE,
                    Texture = textures.Get($"Gameplay/_{Playfield.Manager.Map.KeyCount}k/Receptor/up-{id + 1}")
                },
                sprDown = new Sprite
                {
                    Name = "Sprite Down",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = SIZE,
                    Alpha = 0,
                    Texture = textures.Get($"Gameplay/_{Playfield.Manager.Map.KeyCount}k/Receptor/down-{id + 1}")
                }
            };
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
