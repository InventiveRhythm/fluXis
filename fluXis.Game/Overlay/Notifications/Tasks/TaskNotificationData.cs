using System;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Notifications.Tasks;

public class TaskNotificationData
{
    public string Text { get; set; } = "Running Task...";
    public string TextWorking { get; set; } = "Working...";
    public string TextFinished { get; set; } = "Finished!";
    public string TextFailed { get; set; } = "Failed! View logs for more info.";
    public IconUsage WorkingIcon { get; set; } = FontAwesome6.Solid.ArrowsRotate;
    public bool SpinIcon { get; set; } = true;
    public Action ClickAction { get; set; }

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

    public TaskNotification Create() => new(this);
}
