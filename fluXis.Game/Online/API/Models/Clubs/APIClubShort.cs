using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Online.API.Models.Other;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Logging;

namespace fluXis.Game.Online.API.Models.Clubs;

public class APIClubShort
{
    [JsonProperty("id")]
    public int ID { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("colors")]
    public List<GradientColor> Colors { get; set; }

    public ColourInfo CreateColorInfo()
    {
        try
        {
            Colour4.TryParseHex(Colors.First().Color, out var first);
            Colour4.TryParseHex(Colors.Last().Color, out var last);
            return ColourInfo.GradientHorizontal(first, last);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to create colour info for club tag.");
            return Colour4.White;
        }
    }
}
