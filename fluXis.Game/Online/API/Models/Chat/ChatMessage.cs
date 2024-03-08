using fluXis.Shared.Components.Chat;
using fluXis.Shared.Components.Users;

namespace fluXis.Game.Online.API.Models.Chat;

public class ChatMessage : IChatMessage
{
    public string ID { get; init; } = null!;
    public long CreatedAtUnix { get; init; }
    public string Content { get; init; } = null!;
    public string Channel { get; init; } = null!;
    public APIUserShort Sender { get; init; } = null!;
}
