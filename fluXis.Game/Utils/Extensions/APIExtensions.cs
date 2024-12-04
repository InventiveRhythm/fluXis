using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Online.API.Models.Clubs;
using fluXis.Game.Online.API.Models.Other;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
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
        => CreateColorInfo(club.Colors);

    public static ColourInfo CreateColorInfo(this List<GradientColor> colors)
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
