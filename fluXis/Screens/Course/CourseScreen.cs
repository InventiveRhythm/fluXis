using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Input;
using fluXis.Mods;
using fluXis.Scoring;
using fluXis.Screens.Gameplay;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Screens.Course;

#nullable enable

public partial class CourseScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public List<RealmMap> MapQueue { get; }
    public List<ScoreInfo> Scores { get; } = new();

    private CourseScreenFooter footer = null!;

    private Sample? sampleConfirm;
    private Sample? sampleComplete;
    private Sample? sampleFailed;

    public CourseScreen(List<RealmMap> maps)
    {
        MapQueue = maps;
    }

    [BackgroundDependencyLoader]
    private void load(ISkin skin)
    {
        sampleConfirm = skin.GetCourseSample(SampleType.Confirm);
        sampleComplete = skin.GetCourseSample(SampleType.Complete);
        sampleFailed = skin.GetCourseSample(SampleType.Failed);

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

        sampleConfirm?.Play();

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

        sampleComplete?.Play();

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

    public enum SampleType
    {
        Confirm,
        Complete,
        Failed,
    }
}
