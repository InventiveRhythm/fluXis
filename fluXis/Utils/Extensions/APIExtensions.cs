using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Models.Scores;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Logging;

namespace fluXis.Utils.Extensions;

/// <summary>
/// Extensions for API classes.
/// </summary>
public static class APIExtensions
{
    public static ColourInfo CreateColorInfo(this APIClub club)
        => CreateColorInfo(club.Colors);

    public static ColourInfo CreateColorInfo(this List<APIGradientColor> colors)
    {
        try
        {
            Colour4.TryParseHex(colors.First().Color, out var first);
            Colour4.TryParseHex(colors.Last().Color, out var last);
            return ColourInfo.GradientHorizontal(first, last);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to create colour info.");
            return Colour4.White;
        }
    }

    public static bool IsDeveloper(this APIUser user)
        => user.Groups.Any(g => g.ID == "dev");

    public static bool IsPurifier(this APIUser user)
    {
        if (user.IsDeveloper())
            return true;

        return user.Groups.Any(g => g.ID == "purifier");
    }

    public static bool CanModerate(this APIUser user)
    {
        if (user.IsDeveloper())
            return true;

        return user.Groups.Any(g => g.ID == "moderators");
    }

    //TODO: support dual scores
    public static ScoreInfo ToScoreInfo(this APIScore score)
    {
        ScoreInfo scoreInfo = new ScoreInfo
        {
            MapID = score.Map?.ID ?? -1,
            Timestamp = score.Time,
            Mods = score.Mods.Split(",").ToList(),
        };

        scoreInfo.Players ??= new List<PlayerScore>();
        scoreInfo.Players.Add(new PlayerScore
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
            ScrollSpeed = score.ScrollSpeed,
            PlayerID = score.User.ID
        });

        return scoreInfo;
    }
}
