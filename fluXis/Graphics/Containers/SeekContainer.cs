using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Graphics.Containers;

public partial class SeekContainer : Container
{
    public Action<float> OnSeek { get; set; }
    public Func<bool> IsPlaying { get; set; }

    public bool AlwaysDebounce { get; set; } = false; // debounce regardless of playing state

    public float HorizontalOffset { get; set; } = 0;
    public double DebounceTime { get; set; } = Styling.SEEK_DEBOUNCE;

    public Bindable<float> Progress { get; private set; } = new(0);

    private double? lastSeekTime;

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        seekToMousePosition(e.MousePosition);
        return true;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        seekToMousePosition(e.MousePosition);
        return true;
    }

    protected override void OnDrag(DragEvent e)
    {
        float x = Math.Clamp(e.MousePosition.X - HorizontalOffset, 0, DrawWidth);
        float progress = DrawWidth > 0 ? x / DrawWidth : 0;

        if (!float.IsFinite(progress) || float.IsNaN(progress))
            progress = 0;

        Progress.Value = progress;

        bool shouldDebounce = IsPlaying != null && IsPlaying() && DebounceTime > 0 || AlwaysDebounce;
        
        if (shouldDebounce && lastSeekTime != null && Time.Current - lastSeekTime < DebounceTime)
            return;

        OnSeek?.Invoke(progress);
        lastSeekTime = Time.Current;
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        base.OnDragEnd(e);
        seekToMousePosition(e.MousePosition);
        lastSeekTime = null;
    }

    private void seekToMousePosition(Vector2 position)
    {
        float x = Math.Clamp(position.X - HorizontalOffset, 0, DrawWidth);
        float progress = DrawWidth > 0 ? x / DrawWidth : 0;

        if (!float.IsFinite(progress) || float.IsNaN(progress))
            progress = 0;

        Progress.Value = progress;
        OnSeek?.Invoke(progress);
    }
}
