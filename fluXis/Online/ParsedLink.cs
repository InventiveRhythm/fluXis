using System;
using System.Linq;
using Midori.Utils.Extensions;

namespace fluXis.Online;

/// <summary>
/// a parsed action from a link that gets executed in-game
/// </summary>
public class ParsedLink
{
    public LinkAction Action { get; }
    public object Argument { get; }

    public ParsedLink(LinkAction action, object argument)
    {
        Action = action;
        Argument = argument;
    }

    public static ParsedLink Parse(string link, EndpointConfig endpoints)
    {
        if (!link.StartsWith(endpoints.WebsiteRootUrl))
            return Unknown(link);

        var path = link.Replace(endpoints.WebsiteRootUrl, "");
        var split = path.Split("/", StringSplitOptions.RemoveEmptyEntries);

        if (split.Length < 2)
            return Unknown(link);

        var act = split[0];
        var arg = split[1];

        switch (act)
        {
            case "set":
            {
                if (arg.Split("#").First().TryParseLongInvariant(out var id))
                    return new ParsedLink(LinkAction.MapSet, id);

                break;
            }

            case "u":
            {
                if (arg.Split("#").First().TryParseLongInvariant(out var id))
                    return new ParsedLink(LinkAction.User, id);

                break;
            }

            case "clubs":
            {
                if (arg.Split("#").First().TryParseLongInvariant(out var id))
                    return new ParsedLink(LinkAction.Club, id);

                break;
            }
        }

        return Unknown(link);
    }

    public static ParsedLink Unknown(string link) => new(LinkAction.Unknown, link);
}

public enum LinkAction
{
    /// <summary>
    /// Couldn't parse link, argument is just the original link.
    /// </summary>
    Unknown,

    /// <summary>
    /// [web]/set/:id
    /// </summary>
    MapSet,

    /// <summary>
    /// [web]/u/:id
    /// </summary>
    User,

    /// <summary>
    /// [web]/club/:id
    /// </summary>
    Club
}
