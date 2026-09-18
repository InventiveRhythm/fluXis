using System.Text.RegularExpressions;
using fluXis.Online.Chat.Segments;

namespace fluXis.Online.Chat.Syntax;

public partial class LinkSyntaxMatcher : ISyntaxMatcher
{
    public Regex SegmentMatcher { get; } = segmentRegex();

    public IMessageSegment CreateSegment(ChatDecoManager manager, Match match)
        => new LinkSegment(match.Value);

    [GeneratedRegex(@"(?<Link>https?://[^\s/$.?#].[^\s]*)", RegexOptions.Compiled)]
    private static partial Regex segmentRegex();
}
