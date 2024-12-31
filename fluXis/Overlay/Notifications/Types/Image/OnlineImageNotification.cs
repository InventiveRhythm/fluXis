using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Overlay.Notifications.Types.Image;

public partial class OnlineImageNotification : FloatingImageNotification
{
    public OnlineImageNotification(ImageNotificationData data)
        : base(data)
    {
    }

    protected override Drawable CreateImage() => new OnlineImage(Data.Path);

    private partial class OnlineImage : Sprite
    {
        private readonly string path;
        private readonly int width;

        public OnlineImage(string path, int width = 300)
        {
            this.path = path;
            this.width = width;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            var tex = textures.Get(path);

            if (tex == null) return;

            var ratio = (float)width / tex.Width;

            Width = width;
            Height = tex.Height * ratio;
            Texture = tex;
        }
    }
}
