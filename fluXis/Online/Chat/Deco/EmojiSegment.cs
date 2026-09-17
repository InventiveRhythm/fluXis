namespace fluXis.Online.Chat.Deco;

public class EmojiSegment(string name) : IMessageSegment
{
    public string Name => name;
}
