namespace fluXis.Online.Chat.Segments;

public class LinkSegment(string url) : IMessageSegment
{
    public string Url => url;
}
