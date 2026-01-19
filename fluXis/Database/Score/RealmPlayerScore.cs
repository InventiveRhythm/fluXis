using System;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using Realms;

namespace fluXis.Database.Score;

public class RealmPlayerScore : EmbeddedObject
{
    public RealmScore RealmScore { get; set; } = null!;

    public float Accuracy { get; set; }
    public double PerformanceRating { get; set; }
    public string Grade { get; set; }
    public int Score { get; set; }
    public int MaxCombo { get; set; }
    public int Flawless { get; set; }
    public int Perfect { get; set; }
    public int Great { get; set; }
    public int Alright { get; set; }
    public int Okay { get; set; }
    public int Miss { get; set; }
    public long PlayerID { get; set; }
    public float ScrollSpeed { get; set; }

    [Ignored]
    public ScoreRank Rank
    {
        get => Enum.Parse<ScoreRank>(Grade);
        set => Grade = value.ToString();
    }

    public static RealmPlayerScore FromPlayerScore(PlayerScore playerScore) => new()
    {
        Accuracy = playerScore.Accuracy,
        PerformanceRating = playerScore.PerformanceRating,
        Rank = playerScore.Rank,
        Score = playerScore.Score,
        MaxCombo = playerScore.MaxCombo,
        Flawless = playerScore.Flawless,
        Perfect = playerScore.Perfect,
        Great = playerScore.Great,
        Alright = playerScore.Alright,
        Okay = playerScore.Okay,
        Miss = playerScore.Miss,
        PlayerID = playerScore.PlayerID,
        ScrollSpeed = playerScore.ScrollSpeed
    };

    public PlayerScore ToPlayerScore() => new()
    {
        Accuracy = Accuracy,
        PerformanceRating = PerformanceRating,
        Rank = Rank,
        Score = Score,
        MaxCombo = MaxCombo,
        Flawless = Flawless,
        Perfect = Perfect,
        Great = Great,
        Alright = Alright,
        Okay = Okay,
        Miss = Miss,
        PlayerID = PlayerID,
        ScrollSpeed = ScrollSpeed
    };
}
