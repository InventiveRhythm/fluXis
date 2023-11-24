using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Notifications.Types.Image;

public partial class FileSystemImageNotification : FloatingImageNotification
{
    public FileSystemImageNotification(ImageNotificationData data)
        : base(data)
    {
    }

    protected override Drawable CreateImage() => new FileSystemImage(Data.Path);

    private partial class FileSystemImage : Sprite
    {
        private readonly string path;
        private readonly int width;

        public FileSystemImage(string path, int width = 300)
        {
            this.path = path;
            this.width = width;
        }

        // TODO: actually make a way to load images from the filesystem
        // i didnt make it yet because its not needed yet
        /*[BackgroundDependencyLoader]
        private void load(StorageTextureStore textures)
        {
            var tex = textures.Get(path);

            if (tex == null) return;

            var ratio = (float)width / tex.Width;

            Width = width;
            Height = tex.Height * ratio;
            Texture = tex;
        }*/
    }
}
