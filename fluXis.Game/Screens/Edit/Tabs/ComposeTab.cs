using fluXis.Game.Audio;
using fluXis.Game.Screens.Edit.Tabs.Compose;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class ComposeTab : EditorTab
{
    public ComposeTab(Editor screen)
        : base(screen)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new EditorPlayfield(this)
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
        Conductor.Seek(Conductor.CurrentTime - e.ScrollDelta.Y * 1000);

        return base.OnScroll(e);
    }
}
