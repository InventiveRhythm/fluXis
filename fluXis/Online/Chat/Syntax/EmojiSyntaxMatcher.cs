using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Online.Chat.Segments;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Online.Chat.Syntax;

public partial class EmojiSyntaxMatcher : ISyntaxMatcher
{
    #region Segment

    public Regex SegmentMatcher { get; } = segmentRegex();

    public IMessageSegment CreateSegment(ChatDecoManager manager, Match match)
    {
        var trim = match.Value.Trim(':');

        // return raw text if emoji doesn't exist
        if (!manager.Emojis.Contains(trim))
            return new TextSegment(match.Value);

        return new EmojiSegment(trim);
    }

    [GeneratedRegex("(?<Emoji>:[a-zA-Z0-9_]+:)", RegexOptions.Compiled)]
    private static partial Regex segmentRegex();

    #endregion

    #region Autocomplete

    public Regex AutocompleteRegex { get; } = autocompleteRegex();

    public IEnumerable<MenuItem> AutocompleteSearch(ChatDecoManager manager, string query)
    {
        var matches = manager.Emojis.Where(x => x.StartsWith(query, StringComparison.InvariantCultureIgnoreCase)).Take(24).ToArray();
        return matches.Select(x => new MenuActionItem(x, new IconUsage(manager.EmojiIndexesReversed[x], EmojiIconsStore.ICON_FONT_NAME), () => { }));
    }

    [GeneratedRegex(":([a-zA-Z0-9_]{0,32})$", RegexOptions.Compiled)]
    private static partial Regex autocompleteRegex();

    #endregion
}
