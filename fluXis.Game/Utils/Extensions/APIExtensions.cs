using System;
using System.Linq;
using fluXis.Shared.Components.Clubs;
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
}
