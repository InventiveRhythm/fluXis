namespace fluXis.Online.Chat.Segments;

public class MentionSegment(long id) : IMessageSegment
{
    public long UserID => id;
}
