using fluXis.Game.Overlay.Notifications.Floating;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Notifications.Types.Loading;

public class LoadingNotificationData : INotificationData
{
    public string TextLoading { get; set; } = "Loading...";
    public string TextSuccess { get; set; } = "Loaded!";
    public string TextFailure { get; set; } = "Failed!";
    public IconUsage Icon { get; set; } = FontAwesome.Solid.Sync;

    public BindableFloat ProgressBindable { get; } = new() { MinValue = 0, MaxValue = 1 };
    public Bindable<LoadingState> StateBindable { get; } = new();

    public float Progress
    {
        get => ProgressBindable.Value;
        set => ProgressBindable.Value = value;
    }

    public LoadingState State
    {
        get => StateBindable.Value;
        set => StateBindable.Value = value;
    }

    public FloatingNotification CreateFloating() => new FloatingLoadingNotification(this);
}
