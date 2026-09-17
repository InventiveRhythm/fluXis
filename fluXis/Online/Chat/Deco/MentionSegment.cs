namespace fluXis.Online.Chat.Deco;

public class MentionSegment(long id) : IMessageSegment
{
    public long UserID => id;
}
