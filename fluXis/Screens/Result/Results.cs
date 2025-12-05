using System;
using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Input;
using fluXis.Online;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using fluXis.Screens.Result.Sides.Types;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Screens.Result;

public partial class Results : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private UserCache users { get; set; }

    public override float Zoom => 1.2f;
    public override float BackgroundDim => 0.75f;
    public override float BackgroundBlur => 0.2f;

    public ScoreInfo ComparisonScore { get; set; }

    protected ScoreInfo Score { get; set; }
    public RealmMap Map { get; }
    protected List<APIUser> Players { get; }
    public Bindable<int> SelectedPlayer { get; set; }

    protected DependencyContainer ExtraDependencies { get; set; }

    protected virtual bool PlayEnterAnimation => false;

    public Action OnRestart;
    public Action ViewReplay;

    private ResultsContent content;
    private ResultsFooter footer;

    public Results(RealmMap map, ScoreInfo score)
    {
        Map = map;
        Score = score;
        Players = new List<APIUser>();
        SelectedPlayer = new Bindable<int>(0);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        //maybe doing this here isn't a good idea
        foreach (var scorePlayer in Score.Players)
        {
            Players.Add(users.Get(scorePlayer.PlayerID) ?? APIUser.CreateUnknown(scorePlayer.PlayerID));
        }

        ExtraDependencies.CacheAs(this);
        ExtraDependencies.CacheAs(Score);
        ExtraDependencies.CacheAs(Map);
        ExtraDependencies.CacheAs(Players ?? new List<APIUser> { APIUser.Default });

        InternalChildren = new Drawable[]
        {
            content = new ResultsContent(CreateLeftContent(), CreateRightContent()),
            footer = new ResultsFooter
            {
                BackAction = this.Exit,
                ViewReplayAction = ViewReplay,
                RestartAction = OnRestart
            }
        };
    }

    protected virtual Drawable[] CreateLeftContent()
    {
        var list = new List<Drawable>();

        if (SelectedPlayer.Value < Score.Players.Count && Score.Players[SelectedPlayer.Value].HitResults is not null && Score.Players[SelectedPlayer.Value].HitResults.Count > 0)
            list.Add(new ResultsSideGraph(Score, Map, SelectedPlayer.Value));

        return list.ToArray();
    }

    protected virtual Drawable[] CreateRightContent() => Array.Empty<Drawable>();

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => ExtraDependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected void SetScoreInfo(ScoreInfo score)
    {
        if (score == this.Score) return;

        Score = score;

        Players.Clear();

        foreach (var scorePlayer in Score.Players)
        {
            Players.Add(users.Get(scorePlayer.PlayerID) ?? APIUser.CreateUnknown(scorePlayer.PlayerID));
        }

        //this is what actually reload the screen's content
        if (SelectedPlayer.Value == 0) SelectedPlayer.TriggerChange();
        else SelectedPlayer.Value = 0;
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeOut();

        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
        {
            this.FadeInFromZero(Styling.TRANSITION_FADE);
            content.Show(PlayEnterAnimation);
            footer.Show();
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(Styling.TRANSITION_FADE);
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
