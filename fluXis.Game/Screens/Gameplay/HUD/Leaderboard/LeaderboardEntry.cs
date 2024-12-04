using System;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Scoring;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD.Leaderboard;

public partial class LeaderboardEntry : CompositeDrawable, IComparable<LeaderboardEntry>
{
    protected virtual float TotalScore => score.Score;
    protected virtual double PerformanceRating => score.PerformanceRating;
    protected virtual APIUser Player => users.Get(score.PlayerID) ?? APIUser.CreateUnknown(score.PlayerID);

    [Resolved]
    private UserCache users { get; set; }

    private GameplayLeaderboard leaderboard { get; }
    private ScoreInfo score { get; }

    public float TargetY { get; set; }

    private FluXisSpriteText scoreText { get; set; }

    private float lastScore;
    private double lastPr;

    public LeaderboardEntry(GameplayLeaderboard leaderboard, ScoreInfo score)
    {
        this.leaderboard = leaderboard;
        this.score = score;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(8),
                Children = new Drawable[]
                {
                    new LoadWrapper<DrawableAvatar>
                    {
                        Size = new Vector2(48),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        CornerRadius = 8,
                        Masking = true,
                        LoadContent = () => new DrawableAvatar(Player)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(-2),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                WebFontSize = 20,
                                Text = Player.Username
                            },
                            scoreText = new FluXisSpriteText
                            {
                                WebFontSize = 14,
                                FixedWidth = true
                            }
                        }
                    }
                }
            }
        };

        UpdateValues();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        leaderboard.Mode.BindValueChanged(modeChanged, true);
    }

    private void modeChanged(ValueChangedEvent<GameplayLeaderboardMode> e)
    {
        scoreText.FixedWidth = e.NewValue == GameplayLeaderboardMode.Score;
    }

    protected override void Update()
    {
        base.Update();

        if (Precision.AlmostEquals(TargetY, Y))
            Y = TargetY;
        else
            Y = (float)Interpolation.Lerp(TargetY, Y, Math.Exp(-0.01 * Time.Elapsed));

        if (lastScore != TotalScore || lastPr != PerformanceRating)
            UpdateValues();

        lastScore = TotalScore;
        lastPr = PerformanceRating;
    }

    protected void UpdateValues()
    {
        switch (leaderboard.Mode.Value)
        {
            case GameplayLeaderboardMode.Score:
                scoreText.Text = $"{TotalScore:0000000}";
                break;

            case GameplayLeaderboardMode.Performance:
                scoreText.Text = $"{PerformanceRating.ToStringInvariant("00.00")}pr";
                break;
        }

        leaderboard.PerformSort();
    }

    public int CompareTo(LeaderboardEntry other)
    {
        switch (leaderboard.Mode.Value)
        {
            case GameplayLeaderboardMode.Performance:
            {
                var result = PerformanceRating.CompareTo(other.PerformanceRating);

                if (result != 0)
                    return result;

                break;
            }
        }

        return TotalScore.CompareTo(other.TotalScore);
    }
}
