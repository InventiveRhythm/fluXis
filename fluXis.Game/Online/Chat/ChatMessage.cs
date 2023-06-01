using fluXis.Game.Online.API;

namespace fluXis.Game.Online.Chat;

public class ChatMessage
{
    public APIUserShort Sender { get; set; }
    public string Content { get; set; }
    public string Channel { get; set; }
}
