using Midori.Networking.WebSockets.Typed;

namespace fluXis.Online.Exceptions;

public class InvalidRoomPasswordException : TypedWebSocketException
{
    public InvalidRoomPasswordException(string message)
        : base(message)
    {
    }

    public InvalidRoomPasswordException()
        : base("Invalid password.")
    {
    }
}
