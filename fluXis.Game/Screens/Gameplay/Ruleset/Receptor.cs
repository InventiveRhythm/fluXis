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

        private int currentKeyCount;
        private bool visible;
        private bool isDown;

        public Receptor(Playfield playfield, int id)
        {
            Playfield = playfield;
            currentKeyCount = playfield.Map.InitialKeyCount;
            this.id = id;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Origin = Anchor.BottomCentre;
            Anchor = Anchor.BottomCentre;
            Size = SIZE;
            X = (id - (Playfield.Manager.Map.InitialKeyCount / 2f - .5f)) * SIZE.X;
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

            updateKeyCount(true);
        }

        protected override void Update()
        {
            isDown = Playfield.Screen.Input.Pressed[id];

            if (visible)
            {
                if (isDown)
                {
                    sprDown.Alpha = 1;
                    sprUp.Alpha = 0;
                }
                else
                {
                    sprDown.Alpha = 0;
                    sprUp.Alpha = 1;
                }
            }

            if (currentKeyCount != Playfield.Manager.CurrentKeyCount)
            {
                currentKeyCount = Playfield.Manager.CurrentKeyCount;
                updateKeyCount(false);
            }

            base.Update();
        }

        private void updateKeyCount(bool instant)
        {
            int offset = 0;
            bool prevVisible = visible;

            // Since the current count is the same as the maximum,
            // every receptor should be visible
            if (currentKeyCount == Playfield.Map.KeyCount)
                visible = true;
            else
            {
                bool[][] mode = switchVisibility[Playfield.Map.KeyCount - 5];
                bool[] current = mode[currentKeyCount - 4];
                visible = current[id];

                for (var i = 0; i < current.Length; i++)
                {
                    if (!current[i] && i < id)
                        offset += 1;
                }
            }

            float x = (id - offset - (currentKeyCount / 2f - .5f)) * SIZE.X;

            if (instant)
            {
                X = x;
                this.ScaleTo(visible ? 1 : .9f);

                sprUp.FadeTo(visible ? 1 : 0);
                sprDown.FadeTo(isDown && visible ? 1 : 0);
            }
            else
            {
                if (!prevVisible)
                    this.MoveToX(x);
                else if (visible)
                    this.MoveToX(x, 200, Easing.OutQuint);

                this.ScaleTo(visible ? 1 : .9f, 200, Easing.OutQuint);

                sprUp.FadeTo(visible ? 1 : 0, 200);
                sprDown.FadeTo(isDown && visible ? 1 : 0, 200);
            }
        }

        // all stuff for lane switching
        // theres definitely a better way to do this
        private readonly bool[][][] switchVisibility =
        {
            new[] // 5k
            {
                new[] { true, true, false, true, true }, // 4k
            },
            new[] // 6k
            {
                new[] { false, true, true, true, true, false }, // 4k
                new[] { true, true, true, false, true, true }, // 5k
            },
            new[] // 7k
            {
                new[] { false, true, true, false, true, true, false }, // 4k
                new[] { false, true, true, true, true, true, false }, // 5k
                new[] { true, true, true, false, true, true, true }, // 6k
            }
        };
    }
}
