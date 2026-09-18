using System.Collections.Generic;
using System.Text.RegularExpressions;
using fluXis.Online.Chat.Segments;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Online.Chat.Syntax;

#nullable enable

public interface ISyntaxMatcher
{
    Regex SegmentMatcher { get; }
    IMessageSegment CreateSegment(ChatDecoManager manager, Match match);

    Regex? AutocompleteRegex => null;
    IEnumerable<MenuItem> AutocompleteSearch(ChatDecoManager manager, string query) => [];
}
