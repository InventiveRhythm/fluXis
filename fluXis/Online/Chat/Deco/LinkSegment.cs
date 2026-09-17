namespace fluXis.Online.Chat.Deco;

public class LinkSegment(string url) : IMessageSegment
{
    public string Url => url;
}
