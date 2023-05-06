using System;
using fluXis.Game.Audio;
using fluXis.Game.Screens.Edit.Tabs.Compose;
using fluXis.Game.Screens.Edit.Tabs.Compose.Toolbox;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class ComposeTab : EditorTab
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    private EditorPlayfield playfield;
    private EditorToolbox toolbox;

    public ComposeTab(Editor screen)
        : base(screen)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            playfield = new EditorPlayfield(this),
            toolbox = new EditorToolbox(playfield)
        };
    }

    protected override void Update()
    {
        if (Conductor.CurrentTime > Conductor.Length)
            Conductor.Seek(Conductor.Length);
        if (Conductor.CurrentTime < 0)
            Conductor.Seek(0);

        base.Update();
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

    protected override bool OnScroll(ScrollEvent e)
    {
        int delta = e.ScrollDelta.Y > 0 ? 1 : -1;

        if (e.ControlPressed)
        {
            values.Zoom += delta * .1f;
            values.Zoom = Math.Clamp(values.Zoom, .5f, 5f);
            return true;
        }

        return false;
    }
}
