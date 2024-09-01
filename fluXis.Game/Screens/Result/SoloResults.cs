using System;
using fluXis.Game.Database.Maps;
using fluXis.Game.Input;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Screens.Gameplay;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Result;

public partial class SoloResults : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.2f;
    public override float BackgroundDim => 0.75f;
    public override float BackgroundBlur => 0.2f;

    public bool ForceFromGameplay { get; init; }

    public ScoreInfo Score { get; }
    public RealmMap Map { get; }
    public APIUser Player { get; }

    public ScoreInfo ComparisonScore { get; set; }
    public ScoreSubmitRequest SubmitRequest { get; set; }

    private DependencyContainer dependencies;

    public Action OnRestart;

    private ResultsContent content;
    private ResultsFooter footer;

    public SoloResults(RealmMap map, ScoreInfo score, APIUser player)
    {
        Map = map;
        Score = score;
        Player = player;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        dependencies.CacheAs(this);
        dependencies.CacheAs(Score);
        dependencies.CacheAs(Map);
        dependencies.CacheAs(Player ?? APIUser.Default);

        InternalChildren = new Drawable[]
        {
            content = new ResultsContent(),
            footer = new ResultsFooter
            {
                BackAction = this.Exit,
                RestartAction = OnRestart
            }
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.FadeInFromZero(FADE_DURATION);
            content.Show(e.Last is GameplayScreen || ForceFromGameplay);
            footer.Show();
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(FADE_DURATION);
        content.Hide();
        footer.Hide();
        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
