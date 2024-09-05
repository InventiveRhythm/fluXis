using System;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD.Leaderboard;

public partial class LeaderboardEntry : CompositeDrawable, IComparable<LeaderboardEntry>
{
    protected virtual float TotalScore => score.Score;
    protected virtual APIUser Player => users.Get(score.PlayerID) ?? APIUser.CreateUnknown(score.PlayerID);

    [Resolved]
    private UserCache users { get; set; }

    private GameplayLeaderboard leaderboard { get; }
    private ScoreInfo score { get; }

    public float TargetY { get; set; }

    private FluXisSpriteText scoreText { get; set; }

    private float lastScore;

    public LeaderboardEntry(GameplayLeaderboard leaderboard, ScoreInfo score)
    {
        this.leaderboard = leaderboard;
        this.score = score;
    }

    [BackgroundDependencyLoader]
    private void load(UserCache users)
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

    protected override void Update()
    {
        base.Update();

        if (Precision.AlmostEquals(TargetY, Y))
            Y = TargetY;
        else
            Y = (float)Interpolation.Lerp(TargetY, Y, Math.Exp(-0.01 * Time.Elapsed));

        if (lastScore != TotalScore)
            UpdateValues();

        lastScore = TotalScore;
    }

    protected void UpdateValues()
    {
        scoreText.Text = $"{TotalScore:0000000}";
        leaderboard.PerformSort();
    }

    public int CompareTo(LeaderboardEntry other) => TotalScore.CompareTo(other.TotalScore);
}
