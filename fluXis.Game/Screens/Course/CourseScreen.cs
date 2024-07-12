using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Input;
using fluXis.Game.Mods;
using fluXis.Game.Screens.Gameplay;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Course;

public partial class CourseScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public List<RealmMap> MapQueue { get; }
    public List<ScoreInfo> Scores { get; } = new();

    private CourseScreenFooter footer;

    public CourseScreen(List<RealmMap> maps)
    {
        MapQueue = maps;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            footer = new CourseScreenFooter
            {
                ExitAction = this.Exit,
                ContinueAction = Continue
            }
        };
    }

    public void Continue()
    {
        var idx = Scores.Count;
        var map = MapQueue[idx];
        var mods = new List<IMod>();

        this.Push(new GameplayLoader(map, mods, () =>
        {
            var screen = new CourseGameplayScreen(map, mods);
            screen.OnResults += onMapComplete;
            return screen;
        }));
    }

    private void onMapComplete(ScoreInfo score)
    {
        var idx = Scores.Count;
        var map = MapQueue[idx];

        Scores.Add(score);

        var anyLeft = idx + 1 != MapQueue.Count;

        if (!anyLeft)
            footer.RightButton.FadeOut();
    }

    public override void OnEntering(ScreenTransitionEvent e) => fadeIn();
    public override void OnSuspending(ScreenTransitionEvent e) => fadeOut();
    public override void OnResuming(ScreenTransitionEvent e) => fadeIn();

    public override bool OnExiting(ScreenExitEvent e)
    {
        fadeOut();
        return base.OnExiting(e);
    }

    private void fadeIn()
    {
        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.FadeIn(FADE_DURATION);
            footer.Show();
        }
    }

    private void fadeOut()
    {
        this.FadeOut(FADE_DURATION);
        footer.Hide();
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                footer.LeftButton?.TriggerClick();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
