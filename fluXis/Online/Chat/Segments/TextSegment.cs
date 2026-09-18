namespace fluXis.Online.Chat.Segments;

public class TextSegment(string text) : IMessageSegment
{
    public string Text => text;
}
