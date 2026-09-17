namespace fluXis.Online.Chat.Deco;

public class TextSegment(string text) : IMessageSegment
{
    public string Text => text;
}
