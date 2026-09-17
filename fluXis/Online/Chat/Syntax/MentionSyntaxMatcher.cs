using System.Text.RegularExpressions;
using fluXis.Online.Chat.Segments;
using Midori.Utils.Extensions;

namespace fluXis.Online.Chat.Syntax;

public partial class MentionSyntaxMatcher : ISyntaxMatcher
{
    public Regex SegmentMatcher { get; } = segmentRegex();

    public IMessageSegment CreateSegment(ChatDecoManager manager, Match match)
    {
        var trim = match.Value[2..^1];

        if (trim.TryParseLongInvariant(out var id))
            return new MentionSegment(id);

        return new TextSegment(match.Value);
    }

    [GeneratedRegex(@"(?<Mention>\<@[0-9]+\>)", RegexOptions.Compiled)]
    private static partial Regex segmentRegex();
}
