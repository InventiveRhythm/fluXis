using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Graphics.Background
{
    public class Background : CompositeDrawable
    {
        private readonly string backgroundFile;
        private Sprite backgroundSprite;

        public Background(string path)
        {
            backgroundFile = path;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, BackgroundTextureStore backgrounds)
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(1.1f); // 10% bigger than the screen, used for the Y movement

            // blurring seems like it takes very long to load
            // i'll try to fix this later

            /*InternalChild = new BufferedContainer(cachedFrameBuffer: true)
            {
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false,
                FrameBufferScale = new Vector2(10),
                BlurSigma = new Vector2(100),
                Child = backgroundSprite = new Sprite
                {
                    Texture = backgrounds.Get(backgroundFile) ?? textures.Get(@"Backgrounds/default.png"),
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(1f),
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            };*/

            InternalChild = backgroundSprite = new Sprite
            {
                Texture = backgrounds.Get(backgroundFile) ?? textures.Get(@"Backgrounds/default.png"),
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1f),
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };

            Alpha = 0f;
        }

        protected override void LoadComplete()
        {
            this.MoveToY(.05f)
                .FadeInFromZero(300)
                .MoveToY(0f, 800, Easing.OutQuint);

            base.LoadComplete();
        }
    }
}
