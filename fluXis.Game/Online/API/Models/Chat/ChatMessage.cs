using fluXis.Game.Online.API.Models.Users;

namespace fluXis.Game.Online.API.Models.Chat;

public class ChatMessage : IChatMessage
{
    public string ID { get; init; } = null!;
    public long CreatedAtUnix { get; init; }
    public string Content { get; init; } = null!;
    public string Channel { get; init; } = null!;
    public APIUser Sender { get; init; } = null!;
}
