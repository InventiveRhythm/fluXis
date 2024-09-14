using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Gameplay.HUD.Leaderboard;

public partial class GameplayLeaderboard : Container<LeaderboardEntry>
{
    [Resolved]
    private Playfield playfield { get; set; }

    private Bindable<bool> visible;
    public Bindable<GameplayLeaderboardMode> Mode { get; private set; }

    private List<ScoreInfo> scores { get; }
    private ScoreProcessor processor { get; }

    public GameplayLeaderboard(List<ScoreInfo> scores, ScoreProcessor processor)
    {
        this.scores = scores;
        this.processor = processor;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        visible = config.GetBindable<bool>(FluXisSetting.GameplayLeaderboardVisible);
        Mode = config.GetBindable<GameplayLeaderboardMode>(FluXisSetting.GameplayLeaderboardMode);

        AutoSizeAxes = Axes.X;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        Padding = new MarginPadding(20);
        AlwaysPresent = true;

        InternalChildrenEnumerable = scores.Take(10).Select(s => new LeaderboardEntry(this, s)).OrderDescending();
        AddInternal(new SelfLeaderboardEntry(this, processor));
    }

    public void PerformSort() => SortInternal();

    protected override void Update()
    {
        base.Update();

        updateOpacity();

        var y = 0f;

        foreach (var child in Children)
        {
            child.TargetY = y;
            y += child.DrawHeight + 12;
        }

        Height = y;
    }

    private void updateOpacity()
    {
        // lowers the opacity of the list when the playfield gets close to it
        var alpha = visible.Value ? 1f : 0f;

        if (playfield.RelativePosition <= .4f)
        {
            var val = Math.Max(playfield.RelativePosition - .3f, 0f) / .1f;
            alpha -= .98f * (1 - val);
        }

        alpha = Math.Clamp(alpha, 0, 1);

        if (Precision.AlmostEquals(alpha, Alpha))
            Alpha = alpha;
        else
            Alpha = (float)Interpolation.Lerp(alpha, Alpha, Math.Exp(-0.01 * Time.Elapsed));
    }

    protected override int Compare(Drawable x, Drawable y)
    {
        var a = (LeaderboardEntry)x;
        var b = (LeaderboardEntry)y;
        return -a.CompareTo(b);
    }
}
