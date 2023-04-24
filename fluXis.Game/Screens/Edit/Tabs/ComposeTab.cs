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
    public Action OnTimingPointChanged => () =>
    {
        playfield.RedrawLines();
    };

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
            if (Conductor.IsPlaying)
                Conductor.PauseTrack();
            else
                Conductor.ResumeTrack();

            return true;
        }

        return base.OnKeyDown(e);
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        float time = Conductor.BeatTime / playfield.Snap;
        time = Conductor.CurrentTime + e.ScrollDelta.Y * time;
        time = playfield.SnapTime(time + 10); // +10 to avoid rounding errors
        Conductor.Seek(time, 200, Easing.OutQuint);

        return base.OnScroll(e);
    }
}
