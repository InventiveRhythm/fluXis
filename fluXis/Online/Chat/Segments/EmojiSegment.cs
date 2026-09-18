namespace fluXis.Online.Chat.Segments;

public class EmojiSegment(string name) : IMessageSegment
{
    public string Name => name;
}
