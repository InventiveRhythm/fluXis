using System;
using fluXis.Game.Graphics;
using fluXis.Game.Screens.Edit.Tabs.Compose;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class ComposeTab : EditorTab
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    private LoadingIcon loadingIcon;
    private bool startedLoading;

    public ComposeTab(Editor screen)
        : base(screen)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(50)
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (!startedLoading)
        {
            startedLoading = true;

            LoadComponentAsync(new EditorPlayfield(this), playfield =>
            {
                loadingIcon.FadeOut(200);
                AddInternal(playfield);
                ChangeChildDepth(playfield, -1);
            });
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.Space)
        {
            if (clock.IsRunning)
                clock.Stop();
            else
                clock.Start();

            return true;
        }

        return base.OnKeyDown(e);
    }

    private double accumulation;

    protected override bool OnScroll(ScrollEvent e)
    {
        var scroll = e.ShiftPressed ? e.ScrollDelta.X : e.ScrollDelta.Y;
        int delta = scroll > 0 ? 1 : -1;

        if (e.ControlPressed)
        {
            values.Zoom += delta * .1f;
            values.Zoom = Math.Clamp(values.Zoom, .5f, 5f);
        }
        else if (e.ShiftPressed)
        {
            var snaps = EditorPlayfield.SNAP_DIVISORS;
            var index = Array.IndexOf(snaps, values.SnapDivisor);
            index += delta;

            if (index < 0)
                index = snaps.Length - 1;
            else if (index >= snaps.Length)
                index = 0;

            values.SnapDivisor = snaps[index];
            changeHandler.SnapDivisorChanged?.Invoke();
        }
        else
        {
            if (accumulation != 0 && Math.Sign(accumulation) != delta)
                accumulation = delta * (1 - Math.Abs(accumulation));

            accumulation += e.ScrollDelta.Y;

            while (Math.Abs(accumulation) >= 1)
            {
                seek(accumulation > 0 ? 1 : -1);
                accumulation = accumulation < 0 ? Math.Min(0, accumulation + 1) : Math.Max(0, accumulation - 1);
            }
        }

        return true;
    }

    private void seek(int direction)
    {
        double amount = 1;

        if (clock.IsRunning)
        {
            var tp = values.MapInfo.GetTimingPoint(clock.CurrentTime);
            amount *= 4 * (tp.BPM / 120);
        }

        if (direction < 1)
            clock.SeekBackward(amount);
        else
            clock.SeekForward(amount);
    }
}
