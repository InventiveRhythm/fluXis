using fluXis.Game.Overlay.Notifications.Floating;

namespace fluXis.Game.Overlay.Notifications.Types.Image;

public class ImageNotificationData : INotificationData
{
    public string Text { get; set; } = string.Empty;
    public ImageLocation Location { get; set; } = ImageLocation.FileSystem;
    public string Path { get; set; } = string.Empty;

    public FloatingNotification CreateFloating()
    {
        switch (Location)
        {
            case ImageLocation.Online:
                return new OnlineImageNotification(this);

            case ImageLocation.FileSystem:
                return new FileSystemImageNotification(this);

            default:
                return null;
        }
    }

    public enum ImageLocation
    {
        Online,
        FileSystem
    }
}
