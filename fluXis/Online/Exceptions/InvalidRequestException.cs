using JetBrains.Annotations;
using Midori.Networking.WebSockets.Typed;

namespace fluXis.Online.Exceptions;

public class InvalidRequestException : TypedWebSocketException
{
    public InvalidRequestException([NotNull] string message)
        : base(message)
    {
    }
}
