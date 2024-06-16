using System;
using System.Linq;
using fluXis.Shared.Components.Clubs;
using fluXis.Shared.Components.Scores;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Logging;

namespace fluXis.Game.Utils.Extensions;

/// <summary>
/// Extensions for API classes.
/// </summary>
public static class APIExtensions
{
    public static ColourInfo CreateColorInfo(this APIClub club)
    {
        try
        {
            Colour4.TryParseHex(club.Colors.First().Color, out var first);
            Colour4.TryParseHex(club.Colors.Last().Color, out var last);
            return ColourInfo.GradientHorizontal(first, last);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to create colour info for club tag.");
            return Colour4.White;
        }
    }

    public static ScoreInfo ToScoreInfo(this APIScore score) => new()
    {
        Accuracy = score.Accuracy,
        Rank = (ScoreRank)Enum.Parse(typeof(ScoreRank), score.Rank),
        Score = score.TotalScore,
        PerformanceRating = score.PerformanceRating,
        MaxCombo = score.MaxCombo,
        Flawless = score.FlawlessCount,
        Perfect = score.PerfectCount,
        Great = score.GreatCount,
        Alright = score.AlrightCount,
        Okay = score.OkayCount,
        Miss = score.MissCount,
        MapID = score.Map?.ID ?? -1,
        MapHash = score.Map?.SHA256Hash ?? "",
        ScrollSpeed = score.ScrollSpeed,
        Timestamp = score.Time,
        Mods = score.Mods.Split(",").ToList(),
        PlayerID = score.User.ID
    };
}
