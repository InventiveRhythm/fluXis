using fluXis.Game.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Graphics.Background
{
    public partial class Background : CompositeDrawable
    {
        private readonly string texture;
        private readonly Sprite sprite;
        private readonly BufferedContainer buffer;

        public float Blur
        {
            get => buffer.BlurSigma.X / 25;
            set => buffer.BlurSigma = new Vector2(value * 25);
        }

        public Background(string path)
        {
            texture = path;
            RelativeSizeAxes = Axes.Both;

            AddInternal(buffer = new BufferedContainer(cachedFrameBuffer: true)
            {
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false,
                Child = sprite = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, BackgroundTextureStore backgrounds, FluXisConfig config)
        {
            sprite.Texture = backgrounds.Get(texture) ?? textures.Get(@"Backgrounds/default.png");
            Blur = config.Get<float>(FluXisSetting.BackgroundBlur);
            Alpha = 0f;
        }

        protected override void LoadComplete()
        {
            this.FadeInFromZero(300);
            base.LoadComplete();
        }
    }
}
